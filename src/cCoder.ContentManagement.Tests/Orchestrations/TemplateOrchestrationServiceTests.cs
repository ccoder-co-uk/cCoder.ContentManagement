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

public partial class TemplateOrchestrationServiceTests
{
    private readonly Mock<ITemplateProcessingService> templateProcessingServiceMock;
    private readonly Mock<ITemplateEventProcessingService> templateEventProcessingServiceMock;
    private readonly TemplateOrchestrationService orchestrationService;

    public TemplateOrchestrationServiceTests()
    {
        templateProcessingServiceMock = new Mock<ITemplateProcessingService>(MockBehavior.Strict);
        templateEventProcessingServiceMock = new Mock<ITemplateEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new TemplateOrchestrationService(
            templateProcessingServiceMock.Object,
            templateEventProcessingServiceMock.Object
        );
    }

    private static Template CreateRandomTemplate() => Builder<Template>.CreateNew().Build();
}

























