using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface ISubmissionEventBroker
{
    ValueTask RaiseSubmissionAddEventAsync(EventMessage<Submission> message);

    ValueTask RaiseSubmissionUpdateEventAsync(EventMessage<Submission> message);

    ValueTask RaiseSubmissionDeleteEventAsync(EventMessage<Submission> message);
}
