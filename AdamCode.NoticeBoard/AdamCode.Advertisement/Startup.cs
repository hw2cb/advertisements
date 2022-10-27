using AdamCode.Advertisement.Drivers;
using AdamCode.Advertisement.Services;
using AdamCode.Advertisement.ViewModels;
using Fluid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentFields.Drivers;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Media.Drivers;
using OrchardCore.Media.Fields;
using OrchardCore.Modules;
using OrchardCore.Taxonomies.Fields;
using OrchardCore.Title.Models;
using System;

namespace AdamCode.Advertisement
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<AdvertisementFilterService>();
            services.AddScoped<CategoryService>();

            //services.AddScoped<IContentDisplayDriver, AdvertisementDriver>();
            //services.AddContentField<TextField>().RemoveDisplayDriver<TextFieldDisplayDriver>().UseDisplayDriver<AdvertisementDescriptionDriver>();
            services.AddContentField<MediaField>().UseDisplayDriver<AdvertisementImagesDriver>().RemoveDisplayDriver<MediaFieldDisplayDriver>();

            services.AddContentField<TextField>().ForEditor<AdvertisementDescriptionDriver>();

            services.AddContentPart<TitlePart>().ForEditor<AdvertisementTitleDriver>();

            services.AddContentField<TaxonomyField>().ForEditor<AdvertisementCategoryDriver>();

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name:"Home",
                areaName: "AdamCode.Advertisement",
                pattern:"",
                defaults: new {controller = "Home", action="Index", areaName = "AdamCode.Advertisement" }
            );
        }
    }
}