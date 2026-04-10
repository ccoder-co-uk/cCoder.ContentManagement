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
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Services.Processings;
using Moq;
using IMetadataCache = cCoder.ContentManagement.Exposures.Caching.IMetadataCache;
using JsonBroker = cCoder.ContentManagement.Brokers.JsonBroker;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderComponent = cCoder.Data.Models.CMS.Component;
using RenderComponentParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using RenderConfig = cCoder.ContentManagement.Models.Config;
using RenderResource = cCoder.Data.Models.CMS.Resource;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderUser = cCoder.Data.Models.Security.User;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentRenderProcessingServiceTests
{
    private readonly Mock<IMetadataCache> metadataCacheMock = new();
    private readonly Mock<cCoder.ContentManagement.Exposures.Caching.ICommonObjectCache> commonObjectCacheMock = new();
    private readonly Mock<IRenderFileContentService> renderFileContentServiceMock = new();

    private ComponentRenderProcessingService CreateSut(string workflowBaseUrl) =>
        new(
            metadataCacheMock.Object,
            commonObjectCacheMock.Object,
            new JsonBroker(),
            new RenderConfig
            {
                Settings = new Dictionary<string, string> { ["sslPort"] = "443" },
                Services = new Dictionary<string, string> { ["Workflow"] = workflowBaseUrl },
            },
            renderFileContentServiceMock.Object);

    private static (RenderApp app, RenderUser user, RenderComponent component, RenderComponentParams renderParams) CreateComponentRenderContext()
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
                    Id = 2,
                    AppId = 1,
                    Name = "Child",
                    ResourceKey = "Default",
                    Content = "Child Component",
                    Script = string.Empty,
                },
            ],
            Scripts =
            [
                new RenderScript
                {
                    Id = 3,
                    AppId = 1,
                    Name = "Bootstrap",
                    Content = "bootstrap-script",
                },
            ],
            Resources =
            [
                new RenderResource
                {
                    Id = 4,
                    AppId = 1,
                    Key = "Default",
                    Culture = "en",
                    Name = "Greeting",
                    DisplayName = "Hello",
                    ShortDisplayName = "Hi",
                    Description = "Greeting Description",
                },
            ],
        };

        RenderUser user = new()
        {
            Id = "member",
            DefaultCultureId = "en-GB",
            DisplayName = "Member User",
            Email = "member@app.local",
            Roles = [],
        };

        RenderComponent component = new()
        {
            Id = 10,
            AppId = 1,
            Name = "Hero",
            ResourceKey = "Default",
            Content = string.Join(
                "",
                "[dms[snippets/info]]|",
                "[script[Bootstrap]]|",
                "[component[Child]]|",
                "[meta[site-description]]|",
                "[resource_displayname[Greeting]]|",
                "[theme[Color]]|",
                "[execute]return 'ignored';[/execute]"
            ),
            Script = string.Empty,
        };

        return (app, user, component, new RenderComponentParams("Default", app, user, "en-GB"));
    }
}








