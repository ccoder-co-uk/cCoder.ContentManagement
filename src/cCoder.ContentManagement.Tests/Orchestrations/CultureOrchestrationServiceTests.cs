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

public partial class CultureOrchestrationServiceTests
{
    private readonly Mock<ICultureProcessingService> cultureProcessingServiceMock;
    private readonly Mock<ICultureEventProcessingService> cultureEventProcessingServiceMock;
    private readonly CultureOrchestrationService orchestrationService;

    public CultureOrchestrationServiceTests()
    {
        cultureProcessingServiceMock = new Mock<ICultureProcessingService>(MockBehavior.Strict);
        cultureEventProcessingServiceMock = new Mock<ICultureEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new CultureOrchestrationService(
            cultureProcessingServiceMock.Object,
            cultureEventProcessingServiceMock.Object
        );
    }

    private static Culture CreateRandomCulture() => Builder<Culture>.CreateNew().Build();
}























