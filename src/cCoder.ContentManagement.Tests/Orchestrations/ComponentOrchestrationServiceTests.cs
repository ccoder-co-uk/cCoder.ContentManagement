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

public partial class ComponentOrchestrationServiceTests
{
    private readonly Mock<IComponentProcessingService> componentProcessingServiceMock;
    private readonly Mock<IComponentEventProcessingService> componentEventProcessingServiceMock;
    private readonly ComponentOrchestrationService orchestrationService;

    public ComponentOrchestrationServiceTests()
    {
        componentProcessingServiceMock = new Mock<IComponentProcessingService>(MockBehavior.Strict);
        componentEventProcessingServiceMock = new Mock<IComponentEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new ComponentOrchestrationService(
            componentProcessingServiceMock.Object,
            componentEventProcessingServiceMock.Object
        );
    }

    private static Component CreateRandomComponent() => Builder<Component>.CreateNew().Build();
}


























