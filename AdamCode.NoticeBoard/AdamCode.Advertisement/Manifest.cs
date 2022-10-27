using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "AdamCode.Advertisement",
    Author = "The Orchard Core Team",
    Website = "https://orchardcore.net",
    Version = "0.0.1",
    Description = "AdamCode.Advertisement",
    Category = "AdamCode.Advertisement",
    Dependencies = new[]
    {
        "OrchardCore.Admin",
        "OrchardCore.Apis.GraphQL",
        "OrchardCore.BackgroundTasks",
        "OrchardCore.ContentFields",
        "OrchardCore.Contents",
        "OrchardCore.ContentTypes",
        "OrchardCore.DynamicCache",
        "OrchardCore.Features",
        "OrchardCore.Layers",
        "OrchardCore.Localization",
        "OrchardCore.Media",
        "OrchardCore.Navigation",
        "OrchardCore.Recipes",
        "OrchardCore.Roles",
        "OrchardCore.Settings",
        "OrchardCore.Themes",
        "OrchardCore.Users",
        "OrchardCore.Taxonomies",
        "OrchardCore.Widgets",
        "OrchardCore.ContentPreview"
    }
)]
