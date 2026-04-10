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

public partial class ContentOrchestrationServiceTests
{
    private readonly Mock<IContentProcessingService> contentProcessingServiceMock;
    private readonly Mock<IContentEventProcessingService> contentEventProcessingServiceMock;
    private readonly ContentOrchestrationService orchestrationService;

    public ContentOrchestrationServiceTests()
    {
        contentProcessingServiceMock = new Mock<IContentProcessingService>(MockBehavior.Strict);
        contentEventProcessingServiceMock = new Mock<IContentEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new ContentOrchestrationService(
            contentProcessingServiceMock.Object,
            contentEventProcessingServiceMock.Object
        );
    }

    private static Content CreateRandomContent() => Builder<Content>.CreateNew().Build();
}






















