using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Taxonomies.Drivers;
using OrchardCore.Taxonomies.Fields;
using OrchardCore.Taxonomies.Models;
using OrchardCore.Taxonomies.Settings;
using OrchardCore.Taxonomies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamCode.Advertisement.Drivers
{
    public class AdvertisementCategoryDriver : TaxonomyFieldDisplayDriver
    {
        private readonly IContentManager _contentManager;
        public AdvertisementCategoryDriver(IContentManager contentManager, IStringLocalizer<TaxonomyFieldDisplayDriver> localizer) : base(contentManager, localizer)
        {
            _contentManager = contentManager;   
        }

        public override IDisplayResult Edit(TaxonomyField field, BuildFieldEditorContext context)
        {
            return Initialize<EditTaxonomyFieldViewModel>("Advertisement__Category__Edit", async model =>
            {
                var settings = context.PartFieldDefinition.GetSettings<TaxonomyFieldSettings>();
                model.Taxonomy = await _contentManager.GetAsync(settings.TaxonomyContentItemId, VersionOptions.Latest);

                if (model.Taxonomy != null)
                {
                    var termEntries = new List<TermEntry>();
                    TaxonomyFieldDriverHelper.PopulateTermEntries(termEntries, field, model.Taxonomy.As<TaxonomyPart>().Terms, 0);

                    model.TermEntries = termEntries;
                    model.UniqueValue = termEntries.FirstOrDefault(x => x.Selected)?.ContentItemId;
                }

                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            }).Location("Content:1").OnGroup("User");
        }
    }
}
