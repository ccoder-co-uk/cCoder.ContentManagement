// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class AppSupportingResourcesCoordinationServiceTests
{
    private readonly Mock<IAppCultureOrchestrationService> appCultureOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IScriptOrchestrationService> scriptOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IResourceOrchestrationService> resourceOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly AppSupportingResourcesCoordinationService coordinationService;

    public AppSupportingResourcesCoordinationServiceTests()
    {
        coordinationService = new AppSupportingResourcesCoordinationService(
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

        appCultureOrchestrationServiceMock
            .Setup(expression: service => service.GetAllAppCulture(ignoreFilters: true))
            .Returns(value: new[] { existingCulture }.AsQueryable());

        scriptOrchestrationServiceMock
            .Setup(expression: service => service.GetAllScript(ignoreFilters: true))
            .Returns(value: new[] { existingScript }.AsQueryable());

        resourceOrchestrationServiceMock
            .Setup(expression: service => service.GetAllResource(ignoreFilters: true))
            .Returns(value: new[] { existingResource }.AsQueryable());

        appCultureOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllAppCultureAsync(
deletedAppCulture: It.Is<IEnumerable<AppCulture>>(match: items => items.Single() == existingCulture)))
            .Returns(value: ValueTask.CompletedTask);

        scriptOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllScriptAsync(
deletedScript: It.Is<IEnumerable<Script>>(match: items => items.Single() == existingScript)))
            .Returns(value: ValueTask.CompletedTask);

        resourceOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllResourceAsync(
deletedResource: It.Is<IEnumerable<Resource>>(match: items => items.Single() == existingResource)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await coordinationService.HandleAppUpdateAsync(app: app);

        // Then
        appCultureOrchestrationServiceMock.Verify(
            expression: service => service.GetAllAppCulture(ignoreFilters: true),
            times: Times.Exactly(callCount: 2));

        scriptOrchestrationServiceMock.Verify(
            expression: service => service.GetAllScript(ignoreFilters: true),
            times: Times.Exactly(callCount: 2));

        resourceOrchestrationServiceMock.Verify(
            expression: service => service.GetAllResource(ignoreFilters: true),
            times: Times.Exactly(callCount: 2));

        appCultureOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllAppCultureAsync(
deletedAppCulture: It.Is<IEnumerable<AppCulture>>(match: items => items.Single() == existingCulture)),
times: Times.Once);

        scriptOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllScriptAsync(
deletedScript: It.Is<IEnumerable<Script>>(match: items => items.Single() == existingScript)),
times: Times.Once);

        resourceOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllResourceAsync(
deletedResource: It.Is<IEnumerable<Resource>>(match: items => items.Single() == existingResource)),
times: Times.Once);

        appCultureOrchestrationServiceMock.VerifyNoOtherCalls();
        scriptOrchestrationServiceMock.VerifyNoOtherCalls();
        resourceOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}