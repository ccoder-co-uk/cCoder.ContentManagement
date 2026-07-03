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

    [Fact]
    public async Task ShouldNotTouchSupportingResourceCollectionsWhenHandleAppUpdateAsyncGivenNullCollections()
    {
        // Given
        App app = new()
        {
            Id = 123,
            Cultures = null,
            Scripts = null,
            Resources = null
        };

        // When
        await coordinationService.HandleAppUpdateAsync(app);

        // Then
        appCultureBrokerMock.VerifyNoOtherCalls();
        scriptBrokerMock.VerifyNoOtherCalls();
        resourceBrokerMock.VerifyNoOtherCalls();
        appCultureOrchestrationServiceMock.VerifyNoOtherCalls();
        scriptOrchestrationServiceMock.VerifyNoOtherCalls();
        resourceOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteExistingSupportingResourcesWhenHandleAppUpdateAsyncGivenEmptyCollections()
    {
        // Given
        App app = new()
        {
            Id = 123,
            Cultures = [],
            Scripts = [],
            Resources = []
        };
        AppCulture existingCulture = new() { AppId = app.Id, CultureId = "en" };
        Script existingScript = new() { Id = 456, AppId = app.Id };
        Resource existingResource = new() { Id = 789, AppId = app.Id };

        appCultureBrokerMock
            .Setup(broker => broker.GetAllAppCultures(true))
            .Returns(new[] { existingCulture }.AsQueryable());
        scriptBrokerMock
            .Setup(broker => broker.GetAllScripts(true))
            .Returns(new[] { existingScript }.AsQueryable());
        resourceBrokerMock
            .Setup(broker => broker.GetAllResources(true))
            .Returns(new[] { existingResource }.AsQueryable());
        appCultureBrokerMock
            .Setup(broker => broker.DeleteAllAppCulturesAsync(
                It.Is<IEnumerable<AppCulture>>(items => items.Single() == existingCulture)))
            .Returns(ValueTask.CompletedTask);
        scriptBrokerMock
            .Setup(broker => broker.DeleteAllScriptsAsync(
                It.Is<IEnumerable<Script>>(items => items.Single() == existingScript)))
            .Returns(ValueTask.CompletedTask);
        resourceBrokerMock
            .Setup(broker => broker.DeleteAllResourcesAsync(
                It.Is<IEnumerable<Resource>>(items => items.Single() == existingResource)))
            .Returns(ValueTask.CompletedTask);

        // When
        await coordinationService.HandleAppUpdateAsync(app);

        // Then
        appCultureBrokerMock.Verify(broker => broker.GetAllAppCultures(true), Times.Exactly(2));
        scriptBrokerMock.Verify(broker => broker.GetAllScripts(true), Times.Exactly(2));
        resourceBrokerMock.Verify(broker => broker.GetAllResources(true), Times.Exactly(2));
        appCultureBrokerMock.Verify(
            broker => broker.DeleteAllAppCulturesAsync(
                It.Is<IEnumerable<AppCulture>>(items => items.Single() == existingCulture)),
            Times.Once);
        scriptBrokerMock.Verify(
            broker => broker.DeleteAllScriptsAsync(
                It.Is<IEnumerable<Script>>(items => items.Single() == existingScript)),
            Times.Once);
        resourceBrokerMock.Verify(
            broker => broker.DeleteAllResourcesAsync(
                It.Is<IEnumerable<Resource>>(items => items.Single() == existingResource)),
            Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
        scriptBrokerMock.VerifyNoOtherCalls();
        resourceBrokerMock.VerifyNoOtherCalls();
        appCultureOrchestrationServiceMock.VerifyNoOtherCalls();
        scriptOrchestrationServiceMock.VerifyNoOtherCalls();
        resourceOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}
