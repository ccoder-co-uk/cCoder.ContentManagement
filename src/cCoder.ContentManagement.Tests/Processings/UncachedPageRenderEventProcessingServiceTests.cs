// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class UncachedPageRenderEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseUncachedPageRenderEventAsync()
    {
        // Given
        UncachedPageRenderEvent pageRenderEvent = new() { PageId = 17 };
        Mock<IUncachedPageRenderEventService> eventService = new();

        eventService.Setup(expression: service =>
            service.RaiseUncachedPageRenderEventAsync(
                pageRenderEvent: pageRenderEvent))
            .Returns(value: ValueTask.CompletedTask);

        UncachedPageRenderEventProcessingService service = new(
            eventService: eventService.Object);

        // When
        await service.RaiseUncachedPageRenderEventAsync(
            pageRenderEvent: pageRenderEvent);

        // Then
        eventService.VerifyAll();
    }
}