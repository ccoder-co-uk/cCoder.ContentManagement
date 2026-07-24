// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ISubmissionEventBroker : IAuthenticatedEventBroker
{
    ValueTask RaiseSubmissionAddEventAsync(EventMessage<Submission> message);

    ValueTask RaiseSubmissionUpdateEventAsync(EventMessage<Submission> message);

    ValueTask RaiseSubmissionDeleteEventAsync(EventMessage<Submission> message);
}