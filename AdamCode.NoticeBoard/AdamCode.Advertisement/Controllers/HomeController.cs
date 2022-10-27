using AdamCode.Advertisement.Services;
using AdamCode.Advertisement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using OrchardCore;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Contents;
using OrchardCore.Contents.Services;
using OrchardCore.Contents.ViewModels;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Lists.Models;
using OrchardCore.Navigation;
using OrchardCore.Routing;
using OrchardCore.Settings;
using OrchardCore.Taxonomies.Fields;
using OrchardCore.Taxonomies.Indexing;
using OrchardCore.Taxonomies.Models;
using OrchardCore.Taxonomies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YesSql;
using YesSql.Filters.Query;

namespace AdamCode.Advertisement.Controllers
{
    public class HomeController : Controller
    {
        private const string contentType = "Advertisement";
        private const string contentTypeList = "AdvertisementList";

        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IHtmlLocalizer H;
        private readonly dynamic New;
        private readonly INotifier _notifier;
        private readonly ISession _session;
        private readonly ISiteService _siteService;
        private readonly IUpdateModelAccessor _updateModelAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDisplayManager<ContentOptionsViewModel> _contentOptionsDisplayManager;
        private readonly IOrchardHelper _orchardHelper;
        private readonly AdvertisementFilterService _advertisementFilter;
        private readonly CategoryService _categoryService;
 

        public HomeController(IContentManager contentManager, 
            IContentDefinitionManager contentDefinitionManager, 
            IContentItemDisplayManager contentItemDisplayManager, 
            IHtmlLocalizer<HomeController> h, 
            IShapeFactory shapeFactory, 
            INotifier notifier, 
            ISession session, 
            ISiteService siteService, 
            IUpdateModelAccessor updateModelAccessor,
            IAuthorizationService authorizationService,
            IDisplayManager<ContentOptionsViewModel> contentOptionsDisplayManager,
            IOrchardHelper orchardHelper,
            AdvertisementFilterService filterService,
            CategoryService categoryService)
        {
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _contentItemDisplayManager = contentItemDisplayManager;
            H = h;
            New = shapeFactory;
            _notifier = notifier;
            _session = session;
            _siteService = siteService;
            _updateModelAccessor = updateModelAccessor;
            _authorizationService = authorizationService;
            _contentOptionsDisplayManager = contentOptionsDisplayManager;
            _orchardHelper = orchardHelper;
            _advertisementFilter = filterService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string idCategory, PagerParameters pagerParameters, bool myAdverts = false)
        {
            var siteSettings = await _siteService.GetSiteSettingsAsync();
            var pager = new Pager(pagerParameters, siteSettings.PageSize);



            var maxPagedCount = siteSettings.MaxPagedCount;
            if (maxPagedCount > 0 && pager.PageSize > maxPagedCount)
                pager.PageSize = maxPagedCount;

            var routeData = new RouteData();

            IEnumerable<ContentItem> contentItems = null;
            int countContentItems = 0;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userName = User.Identity.Name;

            if(await _authorizationService.AuthorizeAsync(User, CommonPermissions.EditOwnContent))
            {

                //
                contentItems = await _advertisementFilter.GetAuthAdvertisementsFilterAsync(
                    pager: pager, 
                    userId: userId, 
                    userName: userName, 
                    myAdvertsEnable: myAdverts, 
                    categoryId: idCategory == null ? "" : idCategory);

                countContentItems = await _advertisementFilter.GetAuthAdvertisementsCountFilterAsync(
                    userId: userId,
                    userName: userName,
                    myAdvertsEnable: myAdverts,
                    categoryId: idCategory == null ? "" : idCategory);
            }
            else
            {
                contentItems = await _advertisementFilter.GetAdvertisementsFilterAsync(
                    pager: pager,
                    categoryId: idCategory == null ? "" : idCategory);

                countContentItems = await _advertisementFilter.GetAdvertisementsCountFilterAsync(
                    categoryId: idCategory == null ? "" : idCategory);
            }

            var pagerShape = (await New.Pager(pager)).TotalItemCount(maxPagedCount > 0 ? maxPagedCount : countContentItems).RouteData(routeData);


            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in contentItems)
            {
                contentItemSummaries.Add(await _contentItemDisplayManager.BuildDisplayAsync(contentItem, _updateModelAccessor.ModelUpdater, "Summary"));
            }

            var categories = await _categoryService.GetCategoriesAsync();
            var viewModel = new ListAdvertisementsViewModel
            {
                ContentItems = contentItemSummaries,
                Pager = pagerShape,
                CurrentCategory = idCategory,
                MyAdvertFilter = myAdverts,
                Categories = categories.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = i.Key,
                    Text = i.Value
                })
            };

            return View(viewModel);
        }




        [HttpGet]
        public async Task<IActionResult> Edit(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
                return NotFound();

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) == contentItem.Owner || await _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent))
			{
                var model = await _contentItemDisplayManager.BuildEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, false, "User");

                return View(model);
            }
            else
                return Forbid();


        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public async Task<IActionResult> EditAndPublishPOST(string contentItemId, [Bind(Prefix = "submit.Publish")] string submitPublish, string returnUrl)
        {
            var stayOnSamePage = submitPublish == "submit.PublishAndContinue";
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
                return NotFound();

            return await EditPOST(contentItemId, returnUrl, stayOnSamePage);
        }

        private async Task<IActionResult>EditPOST(string contentItemId, string returnUrl, bool stayOnSamePage)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            if (contentItem == null)
                return NotFound();

            var model = await _contentItemDisplayManager.UpdateEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, false, "User");
            if (!ModelState.IsValid)
            {
                await _session.CancelAsync();
                return View("Edit", model);
            }
            await _contentManager.UnpublishAsync(contentItem);
            _session.Save(contentItem);

            return Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string id = contentType)
        {
            if (String.IsNullOrWhiteSpace(id))
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, CommonPermissions.EditOwnContent))
                return Forbid();

            var contentItem = await _contentManager.NewAsync(id);

            var model = await _contentItemDisplayManager.BuildEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, true, "User");

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public async Task<IActionResult> CreateAndPublishPOST([Bind(Prefix ="submit.Publish")] string submitPublish, string returnUrl, string id = contentType)
        {
            var stayOnSamePage = submitPublish == "submit.PublishAndContinue";

            var dummyContent = await _contentManager.NewAsync(id);

            return await CreatePOST(id, returnUrl, stayOnSamePage);
        }

        private async Task<IActionResult> CreatePOST(string id, string returnUrl, bool stayOnSamePage)
        {
            var contentItem = await _contentManager.NewAsync(id);

            contentItem.Owner = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await _contentItemDisplayManager.UpdateEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, true);

            if (!ModelState.IsValid)
            {
                await _session.CancelAsync();
                return View(nameof(HomeController.Create),model);
            }


            await _contentManager.CreateAsync(contentItem, VersionOptions.Draft);



            if((!string.IsNullOrEmpty(returnUrl)) && (!stayOnSamePage))
            {
                return LocalRedirect(returnUrl);
            }

            return Redirect("/");
        }

        public async Task<IActionResult> Details(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
                return NotFound();

            var model = await _contentItemDisplayManager.BuildDisplayAsync(contentItem, _updateModelAccessor.ModelUpdater);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Publish(string contentItemId, string returnUrl)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
                return NotFound();

            return await PublishPOST(contentItem, returnUrl);
        }
        private async Task<IActionResult> PublishPOST(ContentItem contentItem, string returnUrl)
        {
            if(await _authorizationService.AuthorizeAsync(User, CommonPermissions.PublishContent))
            {
                await _contentManager.PublishAsync(contentItem);
                await _contentManager.UpdateAsync(contentItem);

                return Redirect(returnUrl);
            }
            return Forbid();
        }

		[HttpPost]
        public async Task<IActionResult> Delete(string contentItemId)
		{
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            if (contentItem == null)
                return NotFound();

            if (await _authorizationService.AuthorizeAsync(User, CommonPermissions.DeleteContent) || User.FindFirstValue(ClaimTypes.NameIdentifier) == contentItem.Owner)
			{
                await _contentManager.RemoveAsync(contentItem);

                return Redirect("/");
            }
            else
                return Forbid();
            


		}

		[HttpPost]
        public async Task<IActionResult> SendToModeration(string contentItemId)
		{
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            if (contentItem == null)
                return NotFound();

            if (!(User.IsInRole("Administrator") || User.IsInRole("Moderator")))
                return Forbid();

            await _contentManager.UnpublishAsync(contentItem);

            return RedirectToAction(nameof(Details), new { contentItemId = contentItemId});
        }

    }
}
