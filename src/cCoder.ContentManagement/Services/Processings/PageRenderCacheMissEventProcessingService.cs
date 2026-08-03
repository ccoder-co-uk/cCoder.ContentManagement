// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheMissEventProcessingService(
    IPageRenderCacheMissEventService eventService)
        : IPageRenderCacheMissEventProcessingService
{
    public ValueTask RaisePageRenderCacheMissEventAsync(
        PageRenderCacheMiss cacheMiss) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageRenderCacheMissEventAsync(inputs: [cacheMiss]);

        ValidatePageRenderCacheMiss(
            cacheMiss: cacheMiss,
            parameterName: "cacheMiss");

        return eventService.RaisePageRenderCacheMissEventAsync(
            cacheMiss: cacheMiss);

    }, isValueTask: true);
}