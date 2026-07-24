// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ILayoutEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaiseLayoutAddEventAsync(EventMessage<Layout> message);

    ValueTask RaiseLayoutUpdateEventAsync(EventMessage<Layout> message);

    ValueTask RaiseLayoutDeleteEventAsync(EventMessage<Layout> message);
}