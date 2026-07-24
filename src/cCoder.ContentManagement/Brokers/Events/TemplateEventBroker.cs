// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class TemplateEventBroker(IEventInfrastructureDependency eventInfrastructureDependency)
    : AuthenticatedEventBroker(eventInfrastructureDependency), ITemplateEventBroker
{
    public ValueTask RaiseTemplateAddEventAsync(EventMessage<Template> message) =>
        RaiseEventAsync(name: "template_add", message: message);

    public ValueTask RaiseTemplateUpdateEventAsync(EventMessage<Template> message) =>
        RaiseEventAsync(name: "template_update", message: message);

    public ValueTask RaiseTemplateDeleteEventAsync(EventMessage<Template> message) =>
        RaiseEventAsync(name: "template_delete", message: message);
}