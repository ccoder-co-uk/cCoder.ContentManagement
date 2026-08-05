// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class UncachedPageRenderEventProcessingService(
    IUncachedPageRenderEventService eventService)
        : IUncachedPageRenderEventProcessingService
{
    public ValueTask RaiseUncachedPageRenderEventAsync(
        UncachedPageRenderEvent pageRenderEvent) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseUncachedPageRenderEventAsync(
                inputs: [pageRenderEvent]);

            return eventService.RaiseUncachedPageRenderEventAsync(
                pageRenderEvent: pageRenderEvent);
        }, isValueTask: true);
}