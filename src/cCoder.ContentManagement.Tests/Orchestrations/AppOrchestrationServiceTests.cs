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
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    private readonly Mock<IAppProcessingService> appProcessingServiceMock;
    private readonly Mock<IAppEventProcessingService> appEventProcessingServiceMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly AppOrchestrationService orchestrationService;

    public AppOrchestrationServiceTests()
    {
        appProcessingServiceMock = new Mock<IAppProcessingService>(MockBehavior.Strict);
        appEventProcessingServiceMock = new Mock<IAppEventProcessingService>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        orchestrationService = new AppOrchestrationService(
            appProcessingServiceMock.Object,
            appEventProcessingServiceMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static App CreateRandomApp() => Builder<App>.CreateNew().Build();
}



























