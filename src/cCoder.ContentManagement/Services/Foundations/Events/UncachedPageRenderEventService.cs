// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Models;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal sealed partial class UncachedPageRenderEventService(
    IPageEventBroker pageEventBroker) : IUncachedPageRenderEventService
{
    public ValueTask RaiseUncachedPageRenderEventAsync(
        UncachedPageRenderEvent pageRenderEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateRaiseUncachedPageRenderEventAsync(
                inputs: [pageRenderEvent]);

            EventMessage<UncachedPageRenderEvent> message = new()
            {
                AuthInfo = new EventAuthInfo
                {
                    SSOUserId = pageEventBroker.GetCurrentUserId()
                },
                Data = pageRenderEvent
            };

            await pageEventBroker.RaiseUncachedPageRenderEventAsync(
                message: message);
        }, isValueTask: true);
}