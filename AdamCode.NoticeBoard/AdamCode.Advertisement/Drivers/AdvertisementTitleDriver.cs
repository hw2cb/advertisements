using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Title.Drivers;
using OrchardCore.Title.Models;
using OrchardCore.Title.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamCode.Advertisement.Drivers
{
    public class AdvertisementTitleDriver : TitlePartDisplayDriver
    {
        public AdvertisementTitleDriver(IStringLocalizer<TitlePartDisplayDriver> localizer) : base(localizer)
        {
        }

        public override IDisplayResult Edit(TitlePart titlePart, BuildPartEditorContext context)
        {
            return Initialize<TitlePartViewModel>("Advertisement__Title__Edit", model =>
            {
                model.Title = titlePart.ContentItem.DisplayText;
                model.TitlePart = titlePart;
                model.ContentItem = titlePart.ContentItem;
                model.Settings = context.TypePartDefinition.GetSettings<TitlePartSettings>();
            }).Location("Content:0").OnGroup("User");
        }
    }
}
