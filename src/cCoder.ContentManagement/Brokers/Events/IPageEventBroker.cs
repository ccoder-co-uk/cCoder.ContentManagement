// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaisePageAddEventAsync(EventMessage<Page> message);

    ValueTask RaisePageUpdateEventAsync(EventMessage<Page> message);

    ValueTask RaisePageDeleteEventAsync(EventMessage<Page> message);

    ValueTask RaisePageRenderCacheMissEventAsync(
        EventMessage<PageRenderCacheMiss> message);
}