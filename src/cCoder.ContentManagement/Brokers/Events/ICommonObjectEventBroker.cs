// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ICommonObjectEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaiseCommonObjectAddEventAsync(EventMessage<CommonObject> message);

    ValueTask RaiseCommonObjectUpdateEventAsync(EventMessage<CommonObject> message);

    ValueTask RaiseCommonObjectDeleteEventAsync(EventMessage<CommonObject> message);
}