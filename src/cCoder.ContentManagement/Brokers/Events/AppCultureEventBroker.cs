// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class AppCultureEventBroker(IEventInfrastructureDependency eventInfrastructureDependency)
    : AuthenticatedEventBroker(eventInfrastructureDependency), IAppCultureEventBroker
{
    public ValueTask RaiseAppCultureAddEventAsync(EventMessage<AppCulture> message) =>
        RaiseEventAsync(name: "app_culture_add", message: message);

    public ValueTask RaiseAppCultureDeleteEventAsync(EventMessage<AppCulture> message) =>
        RaiseEventAsync(name: "app_culture_delete", message: message);
}