// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed class AppEventOrchestrationServiceTests
{
    [Fact]
    public async Task RaiseUpdate_WhenCalled_UsesCurrentAuditIdentity()
    {
        // Given
        App app = new() { Id = 42 };
        Mock<IAppEventProcessingService> eventService = new(MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(MockBehavior.Strict);

        authorizationService.Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: "user-id");

        eventService.Setup(expression: service =>
            service.RaiseAppUpdateEventAsync(app, "user-id"))
            .Returns(value: ValueTask.CompletedTask);

        AppOrchestrationService service = new(
            processingService: Mock.Of<IAppProcessingService>(),
            authorizationProcessingService: authorizationService.Object,
            eventProcessingService: eventService.Object);

        // When
        await service.RaiseAppUpdateEventAsync(app: app);

        // Then
        authorizationService.VerifyAll();
        eventService.VerifyAll();
    }
}