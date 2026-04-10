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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    private readonly Mock<IPageProcessingService> pageProcessingServiceMock;
    private readonly Mock<IPageEventProcessingService> pageEventProcessingServiceMock;
    private readonly PageOrchestrationService orchestrationService;

    public PageOrchestrationServiceTests()
    {
        pageProcessingServiceMock = new Mock<IPageProcessingService>(MockBehavior.Strict);
        pageEventProcessingServiceMock = new Mock<IPageEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new PageOrchestrationService(
            pageProcessingServiceMock.Object,
            pageEventProcessingServiceMock.Object
        );
    }

    private static Page CreateRandomPage() => Builder<Page>.CreateNew().Build();
}

























