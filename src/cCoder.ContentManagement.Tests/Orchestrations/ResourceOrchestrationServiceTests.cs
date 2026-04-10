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

public partial class ResourceOrchestrationServiceTests
{
    private readonly Mock<IResourceProcessingService> resourceProcessingServiceMock;
    private readonly Mock<IResourceEventProcessingService> resourceEventProcessingServiceMock;
    private readonly ResourceOrchestrationService orchestrationService;

    public ResourceOrchestrationServiceTests()
    {
        resourceProcessingServiceMock = new Mock<IResourceProcessingService>(MockBehavior.Strict);
        resourceEventProcessingServiceMock = new Mock<IResourceEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new ResourceOrchestrationService(
            resourceProcessingServiceMock.Object,
            resourceEventProcessingServiceMock.Object
        );
    }

    private static Resource CreateRandomResource() => Builder<Resource>.CreateNew().Build();
}






















