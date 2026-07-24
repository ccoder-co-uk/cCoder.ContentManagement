// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class SubmissionEventBroker(IEventInfrastructureDependency eventInfrastructureDependency)
    : AuthenticatedEventBroker(eventInfrastructureDependency), ISubmissionEventBroker
{
    public ValueTask RaiseSubmissionAddEventAsync(EventMessage<Submission> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "submission_add", message: message);

    public ValueTask RaiseSubmissionUpdateEventAsync(EventMessage<Submission> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "submission_update", message: message);

    public ValueTask RaiseSubmissionDeleteEventAsync(EventMessage<Submission> message) =>
        eventInfrastructureDependency.RaiseEventAsync(name: "submission_delete", message: message);
}