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
using FizzWare.NBuilder;
using Moq;
using LocalPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class PageCoordinationServiceTests
{
    private readonly Mock<IPageInfoOrchestrationService> pageInfoOrchestrationServiceMock;
    private readonly Mock<IContentOrchestrationService> contentOrchestrationServiceMock;
    private readonly Mock<IPageRoleOrchestrationService> pageRoleOrchestrationServiceMock;
    private readonly Mock<IPageOrchestrationService> pageOrchestrationServiceMock;
    private readonly PageCoordinationService coordinationService;

    public PageCoordinationServiceTests()
    {
        pageInfoOrchestrationServiceMock = new Mock<IPageInfoOrchestrationService>(
            MockBehavior.Strict
        );
        contentOrchestrationServiceMock = new Mock<IContentOrchestrationService>(
            MockBehavior.Strict
        );
        pageRoleOrchestrationServiceMock = new Mock<IPageRoleOrchestrationService>(
            MockBehavior.Strict
        );
        pageOrchestrationServiceMock = new Mock<IPageOrchestrationService>(MockBehavior.Strict);

        coordinationService = new PageCoordinationService(
            pageInfoOrchestrationServiceMock.Object,
            contentOrchestrationServiceMock.Object,
            pageRoleOrchestrationServiceMock.Object,
            pageOrchestrationServiceMock.Object
        );
    }

    private static Page CreateRandomPage() =>
        Builder<Page>
            .CreateNew()
            .With(page => page.PageInfo = [Builder<PageInfo>.CreateNew().Build()])
            .With(page => page.Contents = [Builder<Content>.CreateNew().Build()])
            .With(page => page.Roles = [Builder<PageRole>.CreateNew().Build()])
            .Build();

    private static LocalPageInfo[] ToLocalPageInfos(IEnumerable<PageInfo> pageInfos) =>
        [.. pageInfos];
}






















