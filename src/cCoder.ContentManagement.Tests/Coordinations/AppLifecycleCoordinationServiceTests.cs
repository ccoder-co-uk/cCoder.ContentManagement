// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Coordinations;

public sealed class AppLifecycleCoordinationServiceTests
{
    [Fact]
    public async Task AddApp_WhenCalled_PersistsParentThenRolesThenRaisesEvent()
    {
        // Given
        App app = new() { Name = "App", Domain = "app.local" };
        List<string> calls = [];
        Mock<IAppOrchestrationService> appService = new(MockBehavior.Strict);
        Mock<IAppBootstrapOrchestrationService> bootstrapService = new(MockBehavior.Strict);
        Mock<IAppRoleOrchestrationService> roleService = new(MockBehavior.Strict);

        appService.Setup(expression: service => service.GetAllApp(true))
            .Returns(value: Array.Empty<App>().AsQueryable());

        bootstrapService
            .Setup(expression: service => service.PrepareNewApp(app, true))
            .Callback(action: () => calls.Add(item: "prepare"))
            .Returns(value: app);

        appService.Setup(expression: service => service.AddAppAsync(app))
            .Callback(action: () => calls.Add(item: "parent"))
            .ReturnsAsync(value: app);

        bootstrapService
            .Setup(expression: service => service.StampAppChildren(app))
            .Callback(action: () => calls.Add(item: "stamp"));

        roleService
            .Setup(expression: service => service.PersistNewAppRolesAsync(app))
            .Callback(action: () => calls.Add(item: "roles"))
            .Returns(value: ValueTask.CompletedTask);

        appService.Setup(expression: service => service.RaiseAppAddEventAsync(app))
            .Callback(action: () => calls.Add(item: "event"))
            .Returns(value: ValueTask.CompletedTask);

        AppLifecycleCoordinationService service = new(
            appOrchestrationService: appService.Object,
            bootstrapOrchestrationService: bootstrapService.Object,
            roleOrchestrationService: roleService.Object);

        // When
        App result = await service.AddAppAsync(newApp: app);

        // Then
        result.Should().BeSameAs(expected: app);
        calls.Should().Equal("prepare", "parent", "stamp", "roles", "event");
        appService.VerifyAll();
        bootstrapService.VerifyAll();
        roleService.VerifyAll();
    }

    [Fact]
    public async Task DeleteApp_WhenAppExists_RaisesEventWithoutDeletingDirectly()
    {
        // Given
        App app = new() { Id = 42 };
        Mock<IAppOrchestrationService> appService = new(MockBehavior.Strict);
        Mock<IAppBootstrapOrchestrationService> bootstrapService = new(MockBehavior.Strict);
        Mock<IAppRoleOrchestrationService> roleService = new(MockBehavior.Strict);

        appService.Setup(expression: service => service.GetAppForDelete(42))
            .Returns(value: app);

        appService.Setup(expression: service => service.RaiseAppDeleteEventAsync(app))
            .Returns(value: ValueTask.CompletedTask);

        AppLifecycleCoordinationService service = new(
            appOrchestrationService: appService.Object,
            bootstrapOrchestrationService: bootstrapService.Object,
            roleOrchestrationService: roleService.Object);

        // When
        await service.DeleteAppAsync(appId: 42);

        // Then
        appService.VerifyAll();
        bootstrapService.VerifyNoOtherCalls();
        roleService.VerifyNoOtherCalls();
    }
}