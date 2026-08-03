// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Models;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal sealed partial class PageRenderCacheMissEventService(
    IPageEventBroker pageEventBroker) : IPageRenderCacheMissEventService
{
    public ValueTask RaisePageRenderCacheMissEventAsync(
        PageRenderCacheMiss cacheMiss) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageRenderCacheMissEventAsync(inputs: [cacheMiss]);

        ValidatePageRenderCacheMiss(
            cacheMiss: cacheMiss,
            parameterName: "cacheMiss");

        EventMessage<PageRenderCacheMiss> message = new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageEventBroker.GetCurrentUserId()
            },
            Data = cacheMiss
        };

        await pageEventBroker.RaisePageRenderCacheMissEventAsync(
            message: message);

    }, isValueTask: true);
}