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
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;
using IPageRenderOrchestrationService = cCoder.ContentManagement.Services.Orchestrations.IPageRenderOrchestrationService;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class PageRenderCoordinationServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IAppOrchestrationService> appOrchestrationServiceMock = new();
    private readonly Mock<ILayoutOrchestrationService> layoutOrchestrationServiceMock = new();
    private readonly Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new();
    private readonly Mock<IResourceOrchestrationService> resourceOrchestrationServiceMock = new();
    private readonly Mock<IComponentOrchestrationService> componentOrchestrationServiceMock = new();
    private readonly Mock<IScriptOrchestrationService> scriptOrchestrationServiceMock = new();
    private readonly Mock<IPageOrchestrationService> pageOrchestrationServiceMock = new();
    private readonly Mock<IContentOrchestrationService> contentOrchestrationServiceMock = new();
    private readonly Mock<IPageInfoOrchestrationService> pageInfoOrchestrationServiceMock = new();
    private readonly Mock<IPageRoleOrchestrationService> pageRoleOrchestrationServiceMock = new();
    private readonly Mock<IPageRenderOrchestrationService> pageRenderOrchestrationServiceMock = new();
    private readonly PageRenderCoordinationService coordinationService;

    public PageRenderCoordinationServiceTests()
    {
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);
        layoutOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Layout>().AsQueryable());
        templateOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Template>().AsQueryable());
        resourceOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Resource>().AsQueryable());
        componentOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Component>().AsQueryable());
        scriptOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Script>().AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Page>().AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(Array.Empty<Page>().AsQueryable());
        contentOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(Array.Empty<Content>().AsQueryable());
        pageInfoOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(Array.Empty<PageInfo>().AsQueryable());
        pageRoleOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(Array.Empty<PageRole>().AsQueryable());

        coordinationService = new PageRenderCoordinationService(
            authorizationBrokerMock.Object,
            appOrchestrationServiceMock.Object,
            layoutOrchestrationServiceMock.Object,
            templateOrchestrationServiceMock.Object,
            resourceOrchestrationServiceMock.Object,
            componentOrchestrationServiceMock.Object,
            scriptOrchestrationServiceMock.Object,
            pageOrchestrationServiceMock.Object,
            contentOrchestrationServiceMock.Object,
            pageInfoOrchestrationServiceMock.Object,
            pageRoleOrchestrationServiceMock.Object,
            pageRenderOrchestrationServiceMock.Object);
    }

    private static App CreateApp() =>
        new()
        {
            Id = 1,
            Name = "Demo",
            Domain = "demo.local",
            DefaultTheme = "Ocean",
            DefaultCultureId = "en-GB",
            ConfigJson = "{}",
            Pages = [],
            Components = [],
            Scripts = [],
            Templates = [],
            Resources = [],
            Layouts =
            [
                new Layout
                {
                    Id = 1,
                    AppId = 1,
                    Name = "Default",
                    HeaderHtml = "<title>[page[title]]</title>",
                    Html = "<main>[content[Body]]</main>",
                    Script = string.Empty
                }
            ]
        };

    private static RenderResult CreateRenderResult(string bodyHtml = "Body") =>
        new()
        {
            HeaderHtml = "Header",
            BodyHtml = bodyHtml,
            Theme = "Ocean",
            Culture = "en-GB",
            Edit = false,
            StatusCode = 200
        };
}







