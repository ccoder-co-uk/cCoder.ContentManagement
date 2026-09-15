// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public sealed partial class CommonObjectEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseImportedEventWithOnlySuccessfulItemsAndCurrentUserAsync()
    {
        // Given
        const string userId = "test-user";
        CommonObject successful = new() { Id = 1 };
        CommonObject failed = new() { Id = 2 };

        OperationResult<CommonObject>[] results =
        [
            new() { Success = true, Item = successful },
            new() { Success = false, Item = failed }
        ];

        Mock<ICommonObjectEventProcessingService> eventService = new(behavior: MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(behavior: MockBehavior.Strict);

        authorizationService
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: userId);

        eventService
            .Setup(expression: service => service.RaiseCommonObjectsImportedEventAsync(
                commonObjects: It.Is<CommonObject[]>(match: items =>
                    items.Length == 1 && items[0] == successful),
                userId: userId))
            .Returns(value: ValueTask.CompletedTask);

        CommonObjectEventOrchestrationService service = new(
            eventProcessingService: eventService.Object,
            authorizationProcessingService: authorizationService.Object);

        // When
        await service.RaiseCommonObjectsImportedEventAsync(results: results);

        // Then
        eventService.VerifyAll();
        authorizationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldNotResolveUserOrRaiseEventWhenNoImportSucceededAsync()
    {
        // Given
        OperationResult<CommonObject>[] results =
            [new() { Success = false, Item = new CommonObject() }];

        Mock<ICommonObjectEventProcessingService> eventService = new(behavior: MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(behavior: MockBehavior.Strict);

        CommonObjectEventOrchestrationService service = new(
            eventProcessingService: eventService.Object,
            authorizationProcessingService: authorizationService.Object);

        // When
        await service.RaiseCommonObjectsImportedEventAsync(results: results);

        // Then
        eventService.VerifyNoOtherCalls();
        authorizationService.VerifyNoOtherCalls();
    }
}