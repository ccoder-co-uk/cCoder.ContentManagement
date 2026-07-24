// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class AppSupportingResourcesCoordinationServiceTests
{
    private readonly Mock<IAppCultureBroker> appCultureBrokerMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IScriptBroker> scriptBrokerMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IResourceBroker> resourceBrokerMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IAppCultureOrchestrationService> appCultureOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IScriptOrchestrationService> scriptOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IResourceOrchestrationService> resourceOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly AppSupportingResourcesCoordinationService coordinationService;

    public AppSupportingResourcesCoordinationServiceTests()
    {
        coordinationService = new AppSupportingResourcesCoordinationService(
appCultureBroker: appCultureBrokerMock.Object,
scriptBroker: scriptBrokerMock.Object,
resourceBroker: resourceBrokerMock.Object,
appCultureOrchestrationService: appCultureOrchestrationServiceMock.Object,
scriptOrchestrationService: scriptOrchestrationServiceMock.Object,
resourceOrchestrationService: resourceOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldPassThroughToSupportingResourceOrchestrationsWhenHandleAppDeleteAsync()
    {
        // Given
        App app = new() { Id = 123 };

        appCultureOrchestrationServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        scriptOrchestrationServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        resourceOrchestrationServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await coordinationService.HandleAppDeleteAsync(app: app);

        // Then
        appCultureOrchestrationServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(appId: app.Id), times: Times.Once);
        scriptOrchestrationServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(appId: app.Id), times: Times.Once);
        resourceOrchestrationServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(appId: app.Id), times: Times.Once);
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
        await coordinationService.HandleAppUpdateAsync(app: app);

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
            .Setup(expression: broker => broker.GetAllAppCultures(ignoreFilters: true))
            .Returns(value: new[] { existingCulture }.AsQueryable());

        scriptBrokerMock
            .Setup(expression: broker => broker.GetAllScripts(ignoreFilters: true))
            .Returns(value: new[] { existingScript }.AsQueryable());

        resourceBrokerMock
            .Setup(expression: broker => broker.GetAllResources(ignoreFilters: true))
            .Returns(value: new[] { existingResource }.AsQueryable());

        appCultureBrokerMock
            .Setup(expression: broker => broker.DeleteAllAppCulturesAsync(
deletedAppCulture: It.Is<IEnumerable<AppCulture>>(match: items => items.Single() == existingCulture)))
            .Returns(value: ValueTask.CompletedTask);

        scriptBrokerMock
            .Setup(expression: broker => broker.DeleteAllScriptsAsync(
deletedScript: It.Is<IEnumerable<Script>>(match: items => items.Single() == existingScript)))
            .Returns(value: ValueTask.CompletedTask);

        resourceBrokerMock
            .Setup(expression: broker => broker.DeleteAllResourcesAsync(
deletedResource: It.Is<IEnumerable<Resource>>(match: items => items.Single() == existingResource)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await coordinationService.HandleAppUpdateAsync(app: app);

        // Then
        appCultureBrokerMock.Verify(expression: broker => broker.GetAllAppCultures(ignoreFilters: true), times: Times.Exactly(callCount: 2));
        scriptBrokerMock.Verify(expression: broker => broker.GetAllScripts(ignoreFilters: true), times: Times.Exactly(callCount: 2));
        resourceBrokerMock.Verify(expression: broker => broker.GetAllResources(ignoreFilters: true), times: Times.Exactly(callCount: 2));

        appCultureBrokerMock.Verify(
expression: broker => broker.DeleteAllAppCulturesAsync(
deletedAppCulture: It.Is<IEnumerable<AppCulture>>(match: items => items.Single() == existingCulture)),
times: Times.Once);

        scriptBrokerMock.Verify(
expression: broker => broker.DeleteAllScriptsAsync(
deletedScript: It.Is<IEnumerable<Script>>(match: items => items.Single() == existingScript)),
times: Times.Once);

        resourceBrokerMock.Verify(
expression: broker => broker.DeleteAllResourcesAsync(
deletedResource: It.Is<IEnumerable<Resource>>(match: items => items.Single() == existingResource)),
times: Times.Once);

        appCultureBrokerMock.VerifyNoOtherCalls();
        scriptBrokerMock.VerifyNoOtherCalls();
        resourceBrokerMock.VerifyNoOtherCalls();
        appCultureOrchestrationServiceMock.VerifyNoOtherCalls();
        scriptOrchestrationServiceMock.VerifyNoOtherCalls();
        resourceOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}