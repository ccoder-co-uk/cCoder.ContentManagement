// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IPageInfoEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaisePageInfoAddEventAsync(EventMessage<PageInfo> message);

    ValueTask RaisePageInfoUpdateEventAsync(EventMessage<PageInfo> message);

    ValueTask RaisePageInfoDeleteEventAsync(EventMessage<PageInfo> message);
}