using System.Text.Json;
using OrchardCore;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using OrchardCore.Taxonomies.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesSql;
using AdamCode.Advertisement.Services.Utility;
using OrchardCore.Queries;
using System.Security.Claims;

namespace AdamCode.Advertisement.Services
{
	public class AdvertisementFilterService
    {
        
        /* Case 1 - Все опубликованные объявления
         * Case 2 - Все опубликованные и на модерации (для владельца)
         * Case 3 - Все опубликованные но определенной категории
         * Case 4 - Все опубликованные и на модерации (для владельца) но определенной категории
         */
        private readonly IOrchardHelper _orchardHelper;
        private readonly IContentManager _contentManager;
        public AdvertisementFilterService(IOrchardHelper orchardHelper, IContentManager contentManager)
        {
            _orchardHelper = orchardHelper;
            _contentManager = contentManager;
        }

        #region Query

        public async Task<IEnumerable<ContentItem>> GetAuthAdvertisementsFilterAsync(Pager pager, string userId, string userName, bool myAdvertsEnable = false, string categoryId = "")
        {
            string filterId = "", filterName = "";
            if(myAdvertsEnable)
            {
                filterId = userId;
                filterName = userName;
            }
            var dataFromDb = await _orchardHelper
                .ContentQueryAsync("CommonAuthQuery",
                    new Dictionary<string, object>
                    {
                        {"categoryId", categoryId },
                        {"userId", userId},
                        {"UserName", userName},
                        {"filterId", filterId},
                        {"filterName", filterName },
                        {"take", pager.PageSize },
                        {"skip", pager.GetStartIndex() }
                    });
            return await LoadContentItemsAsync(dataFromDb);
        }

        public async Task<IEnumerable<ContentItem>> GetAdvertisementsFilterAsync(Pager pager, string categoryId = "")
        {
            var dataFromDb = await _orchardHelper
                .ContentQueryAsync("CommonNotAuthQuery",
                    new Dictionary<string, object>
                    {
                        {"categoryId", categoryId },
                        {"take", pager.PageSize },
                        {"skip", pager.GetStartIndex() }
                    });
            return await LoadContentItemsAsync(dataFromDb);
        }

        private async Task<IEnumerable<ContentItem>> LoadContentItemsAsync(IEnumerable<ContentItem> contentItems)
        {
            List<ContentItem> loadedContentItems = new List<ContentItem>();
            foreach (var contentItem in contentItems)
            {
                loadedContentItems.Add(await _contentManager.GetAsync(contentItem.ContentItemId, VersionOptions.Latest));
            }
            return loadedContentItems;
        }
        #endregion


        #region Count Query

        public async Task<int> GetAuthAdvertisementsCountFilterAsync(string userId, string userName, bool myAdvertsEnable = false, string categoryId = "")
        {
            string filterId = "", filterName = "";
            if (myAdvertsEnable)
            {
                filterId = userId;
                filterName = userName;
            }
            var result = await _orchardHelper
                .QueryResultsAsync("CommonAuthCountQuery",
                    new Dictionary<string, object>
                    {
                        {"categoryId", categoryId },
                        {"userId", userId},
                        {"UserName", userName},
                        {"filterId", filterId},
                        {"filterName", filterName }
                    });
            return GetCount(result);
        }
        public async Task<int> GetAdvertisementsCountFilterAsync(string categoryId = "")
        {
            var result = await _orchardHelper
                .QueryResultsAsync("CommonNotAuthCountQuery",
                    new Dictionary<string, object>
                    {
                        {"categoryId", categoryId }
                    });
            return GetCount(result);
        }
        private int GetCount(IQueryResults queryResult)
        {
            var jsonResult = queryResult.Items.FirstOrDefault().ToString();
            CountResult count = JsonSerializer.Deserialize<CountResult>(jsonResult);
            return count.Count;
        }
        #endregion


    }
}
