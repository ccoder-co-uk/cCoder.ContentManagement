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

public sealed class CommonObjectEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseImportedEventWithOnlySuccessfulItemsAndCurrentUserAsync()
    {
        const string userId = "test-user";
        CommonObject successful = new() { Id = 1 };
        CommonObject failed = new() { Id = 2 };
        OperationResult<CommonObject>[] results =
        [
            new() { Success = true, Item = successful },
            new() { Success = false, Item = failed }
        ];
        Mock<ICommonObjectEventProcessingService> eventService = new(MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(MockBehavior.Strict);
        authorizationService.Setup(service => service.GetCurrentUserId()).Returns(userId);
        eventService.Setup(service => service.RaiseCommonObjectsImportedEventAsync(
                It.Is<CommonObject[]>(items => items.Length == 1 && items[0] == successful),
                userId))
            .Returns(ValueTask.CompletedTask);
        CommonObjectEventOrchestrationService service = new(
            eventService.Object,
            authorizationService.Object);

        await service.RaiseCommonObjectsImportedEventAsync(results);

        eventService.VerifyAll();
        authorizationService.VerifyAll();
    }

    [Fact]
    public async Task ShouldNotResolveUserOrRaiseEventWhenNoImportSucceededAsync()
    {
        OperationResult<CommonObject>[] results =
            [new() { Success = false, Item = new CommonObject() }];
        Mock<ICommonObjectEventProcessingService> eventService = new(MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(MockBehavior.Strict);
        CommonObjectEventOrchestrationService service = new(
            eventService.Object,
            authorizationService.Object);

        await service.RaiseCommonObjectsImportedEventAsync(results);

        eventService.VerifyNoOtherCalls();
        authorizationService.VerifyNoOtherCalls();
    }
}