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

public partial class PageInfoOrchestrationServiceTests
{
    private readonly Mock<IPageInfoProcessingService> pageInfoProcessingServiceMock;
    private readonly Mock<IPageInfoEventProcessingService> pageInfoEventProcessingServiceMock;
    private readonly PageInfoOrchestrationService orchestrationService;

    public PageInfoOrchestrationServiceTests()
    {
        pageInfoProcessingServiceMock = new Mock<IPageInfoProcessingService>(MockBehavior.Strict);
        pageInfoEventProcessingServiceMock = new Mock<IPageInfoEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new PageInfoOrchestrationService(
            pageInfoProcessingServiceMock.Object,
            pageInfoEventProcessingServiceMock.Object
        );
    }

    private static PageInfo CreateRandomPageInfo() => Builder<PageInfo>.CreateNew().Build();
}























