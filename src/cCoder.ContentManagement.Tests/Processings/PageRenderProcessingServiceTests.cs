// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Dependencies.Rendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Orchestrations;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Brokers.ServiceProviders;
using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using cCoder.ContentManagement.Services.Processings;
using Moq;
using JsonBroker = cCoder.ContentManagement.Brokers.JsonBroker;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderComponent = cCoder.Data.Models.CMS.Component;
using RenderConfig = cCoder.ContentManagement.Models.Config;
using RenderContent = cCoder.Data.Models.CMS.Content;
using RenderLayout = cCoder.Data.Models.CMS.Layout;
using RenderPage = cCoder.Data.Models.CMS.Page;
using RenderPageInfo = cCoder.Data.Models.CMS.PageInfo;
using RenderResource = cCoder.Data.Models.CMS.Resource;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderUser = cCoder.Data.Models.Security.User;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRenderProcessingServiceTests
{
    private readonly TestMetadataReaderBroker metadataReaderBroker = new();
    private readonly Mock<ICommonObjectReaderBroker> commonObjectReaderBrokerMock =
        CreateCommonObjectReaderBrokerMock();
    private readonly TestComponentReaderBroker componentReaderBroker = new();
    private readonly TestScriptReaderBroker scriptReaderBroker = new();
    private readonly Mock<IRenderFileContentBroker> renderFileContentBrokerMock = new();

    private PageRenderProcessingService CreateSut(RenderConfig config)
    {
        PageRenderExecutionOrchestrationService executionOrchestrationService =
            new(
                metadataCacheService: new MetadataCacheService(
                    broker: metadataReaderBroker),
                commonObjectCacheService: new CommonObjectCacheService(
                    broker: commonObjectReaderBrokerMock.Object),
                markupRenderService: new MarkupRenderService(
                    componentReaderBroker: componentReaderBroker,
                    scriptReaderBroker: scriptReaderBroker,
                    jsonBroker: new JsonBroker(),
                    renderFileContentBroker: renderFileContentBrokerMock.Object));

        Mock<IServiceProviderBroker> serviceProviderBrokerMock = new();

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetRequiredService<IPageRenderExecutionOrchestrationService>(
                    name: "PageRenderExecution"))
            .Returns(value: executionOrchestrationService);

        PageRenderService pageRenderService =
            new(
                serviceProviderBroker: serviceProviderBrokerMock.Object);

        return new PageRenderProcessingService(
            pageRenderService: pageRenderService,
            config: config);
    }

    private static RenderConfig CreateConfig(string workflowBaseUrl) =>
        new()
        {
            Settings = new Dictionary<string, string> { ["sslPort"] = "443" },
            Services = new Dictionary<string, string> { ["Workflow"] = workflowBaseUrl },
        };

    private static RenderApp CreateApp()
    {
        RenderApp app = new()
        {
            Id = 1,
            Name = "App",
            Domain = "app.local",
            DefaultCultureId = "en-GB",
            DefaultTheme = "Default",
            ConfigJson = "{\"Themes\":{\"Default\":{\"Color\":\"Blue\"}}}",
            Layouts = [],
            Pages = [],
            Templates = [],
            Components = [],
            Scripts = [],
            Resources = [],
        };

        RenderPage home = new()
        {
            Id = 10,
            AppId = app.Id,
            Name = "Home",
            Path = string.Empty,
            Layout = "Default",
            ShowOnMenus = true,
            App = app,
            PageInfo =
            [
                new RenderPageInfo
                {
                    PageId = 10,
                    CultureId = "en-GB",
                    Title = "Home",
                    Description = "Home Description",
                    Keywords = "alpha,beta",
                },
            ],
            Contents =
            [
                new RenderContent
                {
                    Id = 1,
                    PageId = 10,
                    CultureId = "en-GB",
                    Name = "body",
                    Html = "Body Content",
                },
            ],
            Roles = [],
            Pages = [],
        };

        RenderPage summary = new()
        {
            Id = 11,
            ParentId = 10,
            AppId = app.Id,
            Name = "Summary",
            Path = "Summary",
            Layout = "Default",
            ShowOnMenus = true,
            App = app,
            PageInfo =
            [
                new RenderPageInfo
                {
                    PageId = 11,
                    CultureId = "en-GB",
                    Title = "Summary",
                    Description = "Summary Description",
                    Keywords = "summary",
                },
            ],
            Contents = [],
            Roles = [],
            Pages = [],
        };

        app.Pages =
        [
            home,
            summary,
        ];

        app.Layouts =
        [
            new RenderLayout
            {
                Id = 1,
                AppId = app.Id,
                Name = "Default",
                HeaderHtml = "<title>[page[title]]</title><meta>[meta[site-description]]</meta><script>[script[Bootstrap]]</script>",
                Html = string.Join(
                separator: "",
                value:
                [
                    "<nav>[nav[0]]</nav>",
                    "<nav class='expanded'>[navExpanded[0]]</nav>",
                    "<main>[content[Body]]</main>",
                    "<aside>[component[Hero]]</aside>",
                    "<section>[resource_displayname[Greeting]]|[resource_shortdisplayname[Greeting]]|[resource_description[Greeting]]</section>",
                    "<section>[theme[Color]]|[app[name]]|[page[path]]</section>",
                    "<section>[execute]return 'ignored';[/execute]</section>"
                ]),
                Script = string.Empty,
            },
        ];

        app.Components =
        [
            new RenderComponent
            {
                Id = 4,
                AppId = app.Id,
                Name = "Hero",
                ResourceKey = "Default",
                Content = "Hero Component",
                Script = "hero-component-script",
            },
        ];

        app.Scripts =
        [
            new RenderScript
            {
                Id = 5,
                AppId = app.Id,
                Name = "Bootstrap",
                Content = "bootstrap-script",
            },
        ];

        app.Resources =
        [
            new RenderResource
            {
                Id = 6,
                AppId = app.Id,
                Key = "Default",
                Culture = "en",
                Name = "Greeting",
                DisplayName = "Hello",
                ShortDisplayName = "Hi",
                Description = "Greeting Description",
            },
            new RenderResource
            {
                Id = 7,
                AppId = app.Id,
                Key = "Default",
                Culture = "en",
                Name = "Logout",
                DisplayName = "Logout",
                ShortDisplayName = "Logout",
                Description = "Logout",
            },
        ];

        return app;
    }

    private static RenderUser CreateUser() =>
        new()
        {
            Id = "member",
            DefaultCultureId = "en-GB",
            DisplayName = "Member User",
            Email = "member@app.local",
            Roles = [],
        };

    private sealed class TestMetadataReaderBroker : IMetadataReaderBroker
    {
        private readonly Dictionary<string, string> values = new(comparer: StringComparer.OrdinalIgnoreCase);

        public IList<(string Name, string Culture)> Requests { get; } = [];

        public void Set(string name, string culture, string value) =>
            values[BuildKey(name: name, culture: culture)] = value;

        public string Get(string key, string culture) =>
            GetMetadata(name: key, culture: culture);

        public string GetMetadata(string name, string culture)
        {
            Requests.Add(item: (name, culture));

            return values.TryGetValue(key: BuildKey(name: name, culture: culture), value: out string value)
                ? value
                : string.Empty;
        }

        private static string BuildKey(string name, string culture) =>
            $"{name}|{culture}";
    }

    private static Mock<ICommonObjectReaderBroker> CreateCommonObjectReaderBrokerMock()
    {
        IReadOnlyDictionary<string, PageRenderResource> resourcesByLookup =
            new Dictionary<string, PageRenderResource>(comparer: StringComparer.OrdinalIgnoreCase);

        IReadOnlyDictionary<string, PageRenderComponent> componentsByName =
            new Dictionary<string, PageRenderComponent>(comparer: StringComparer.OrdinalIgnoreCase);

        IReadOnlyDictionary<string, PageRenderScript> scriptsByName =
            new Dictionary<string, PageRenderScript>(comparer: StringComparer.OrdinalIgnoreCase);

        Mock<ICommonObjectReaderBroker> brokerMock = new();

        brokerMock
            .Setup(expression: broker => broker.GetResourcesByLookup())
            .Returns(value: resourcesByLookup);

        brokerMock
            .Setup(expression: broker => broker.GetComponentsByName())
            .Returns(value: componentsByName);

        brokerMock
            .Setup(expression: broker => broker.GetScriptsByName())
            .Returns(value: scriptsByName);

        return brokerMock;
    }

    private sealed class TestComponentReaderBroker : IComponentReaderBroker
    {
        public IEnumerable<cCoder.Data.Models.CMS.Component> GetComponents(int appId) =>
            Array.Empty<cCoder.Data.Models.CMS.Component>();

        public cCoder.Data.Models.CMS.Component GetComponent(int appId, string name) =>
            null;
    }

    private sealed class TestScriptReaderBroker : IScriptReaderBroker
    {
        public IEnumerable<cCoder.Data.Models.CMS.Script> GetScripts(int appId) =>
            Array.Empty<cCoder.Data.Models.CMS.Script>();

        public cCoder.Data.Models.CMS.Script GetScript(int appId, string name) =>
            null;
    }
}