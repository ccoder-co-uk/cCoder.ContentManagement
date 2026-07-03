using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public class AppSupportingResourcesCoordinationServiceTests
{
    private readonly Mock<IAppCultureBroker> appCultureBrokerMock = new(MockBehavior.Strict);
    private readonly Mock<IScriptBroker> scriptBrokerMock = new(MockBehavior.Strict);
    private readonly Mock<IResourceBroker> resourceBrokerMock = new(MockBehavior.Strict);
    private readonly Mock<IAppCultureOrchestrationService> appCultureOrchestrationServiceMock = new(MockBehavior.Strict);
    private readonly Mock<IScriptOrchestrationService> scriptOrchestrationServiceMock = new(MockBehavior.Strict);
    private readonly Mock<IResourceOrchestrationService> resourceOrchestrationServiceMock = new(MockBehavior.Strict);
    private readonly AppSupportingResourcesCoordinationService coordinationService;

    public AppSupportingResourcesCoordinationServiceTests()
    {
        coordinationService = new AppSupportingResourcesCoordinationService(
            appCultureBrokerMock.Object,
            scriptBrokerMock.Object,
            resourceBrokerMock.Object,
            appCultureOrchestrationServiceMock.Object,
            scriptOrchestrationServiceMock.Object,
            resourceOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldPassThroughToSupportingResourceOrchestrationsWhenHandleAppDeleteAsync()
    {
        // Given
        App app = new() { Id = 123 };
        appCultureOrchestrationServiceMock
            .Setup(service => service.DeleteByAppIdAsync(app.Id))
            .Returns(ValueTask.CompletedTask);
        scriptOrchestrationServiceMock
            .Setup(service => service.DeleteByAppIdAsync(app.Id))
            .Returns(ValueTask.CompletedTask);
        resourceOrchestrationServiceMock
            .Setup(service => service.DeleteByAppIdAsync(app.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await coordinationService.HandleAppDeleteAsync(app);

        // Then
        appCultureOrchestrationServiceMock.Verify(service => service.DeleteByAppIdAsync(app.Id), Times.Once);
        scriptOrchestrationServiceMock.Verify(service => service.DeleteByAppIdAsync(app.Id), Times.Once);
        resourceOrchestrationServiceMock.Verify(service => service.DeleteByAppIdAsync(app.Id), Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        scriptBrokerMock.VerifyNoOtherCalls();
        resourceBrokerMock.VerifyNoOtherCalls();
        appCultureOrchestrationServiceMock.VerifyNoOtherCalls();
        scriptOrchestrationServiceMock.VerifyNoOtherCalls();
        resourceOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}
