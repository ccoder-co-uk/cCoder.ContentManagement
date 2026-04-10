using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Services.Processings;
using Moq;
using IMetadataCache = cCoder.ContentManagement.Exposures.Caching.IMetadataCache;
using JsonBroker = cCoder.ContentManagement.Brokers.JsonBroker;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderComponent = cCoder.Data.Models.CMS.Component;
using RenderConfig = cCoder.ContentManagement.Models.Config;
using RenderResource = cCoder.Data.Models.CMS.Resource;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderTemplate = cCoder.Data.Models.CMS.Template;
using RenderUser = cCoder.Data.Models.Security.User;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateRenderProcessingServiceTests
{
    private readonly Mock<IMetadataCache> metadataCacheMock = new();
    private readonly Mock<cCoder.ContentManagement.Exposures.Caching.ICommonObjectCache> commonObjectCacheMock = new();

    private TemplateRenderProcessingService CreateSut() =>
        new(metadataCacheMock.Object, commonObjectCacheMock.Object, new JsonBroker());

    private static RenderConfig CreateConfig(string workflowBaseUrl) =>
        new()
        {
            Settings = new Dictionary<string, string> { ["sslPort"] = "443" },
            Services = new Dictionary<string, string> { ["Workflow"] = workflowBaseUrl },
        };

    private static (RenderApp app, RenderUser user, RenderTemplate template) CreateTemplateRenderContext()
    {
        RenderApp app = new()
        {
            Id = 1,
            Name = "App",
            Domain = "app.local",
            DefaultCultureId = "en-GB",
            DefaultTheme = "Default",
            ConfigJson = "{\"Themes\":{\"Default\":{\"Color\":\"Blue\"}}}",
            Components =
            [
                new RenderComponent
                {
                    Id = 1,
                    AppId = 1,
                    Name = "Hero",
                    ResourceKey = "Default",
                    Content = "Hero [model[Name]]",
                    Script = string.Empty,
                },
            ],
            Scripts =
            [
                new RenderScript
                {
                    Id = 2,
                    AppId = 1,
                    Name = "Bootstrap",
                    Content = "bootstrap-script",
                },
            ],
            Resources =
            [
                new RenderResource
                {
                    Id = 3,
                    AppId = 1,
                    Key = "Default",
                    Culture = "en",
                    Name = "Greeting",
                    DisplayName = "Hello",
                    ShortDisplayName = "Hi",
                    Description = "Greeting Description",
                },
            ],
            Templates = [],
        };

        RenderUser user = new()
        {
            Id = "member",
            DefaultCultureId = "en-GB",
            DisplayName = "Member User",
            Email = "member@app.local",
            Roles = [],
        };

        RenderTemplate template = new()
        {
            Id = 10,
            AppId = 1,
            Name = "Welcome",
            ResourceKey = "Default",
            RawString = string.Join(
                "",
                "[app[name]]|",
                "[theme[Color]]|",
                "[model[Name]]|",
                "[script[Bootstrap]]|",
                "[component[Hero]]|",
                "[meta[site-description]]|",
                "[resource_displayname[Greeting]]|",
                "[execute]return 'ignored';[/execute]"
            ),
        };

        return (app, user, template);
    }
}







