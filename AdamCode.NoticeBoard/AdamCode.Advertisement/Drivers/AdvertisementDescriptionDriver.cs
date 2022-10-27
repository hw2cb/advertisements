using Microsoft.Extensions.Localization;
using OrchardCore.ContentFields.Drivers;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamCode.Advertisement.Drivers
{
    public class AdvertisementDescriptionDriver : TextFieldDisplayDriver
    {
        public AdvertisementDescriptionDriver(IStringLocalizer<TextFieldDisplayDriver> localizer) : base(localizer)
        {
        }

        public override IDisplayResult Edit(TextField field, BuildFieldEditorContext context)
        {
            return Initialize<EditTextFieldViewModel>("Advertisement__Description__Edit", model =>
            {
                model.Text = field.Text;
                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            }).Location("Content:2").OnGroup("User");
        }
    }
}
