// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IAppCultureEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaiseAppCultureAddEventAsync(EventMessage<AppCulture> message);

    ValueTask RaiseAppCultureDeleteEventAsync(EventMessage<AppCulture> message);
}