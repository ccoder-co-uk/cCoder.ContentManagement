using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public class SubmissionEventBroker(IEventHub eventHub) : ISubmissionEventBroker
{
    public ValueTask RaiseSubmissionAddEventAsync(EventMessage<Submission> message)
    {
        return eventHub.RaiseEventAsync("submission_add", message);
    }

    public ValueTask RaiseSubmissionUpdateEventAsync(EventMessage<Submission> message)
    {
        return eventHub.RaiseEventAsync("submission_update", message);
    }

    public ValueTask RaiseSubmissionDeleteEventAsync(EventMessage<Submission> message)
    {
        return eventHub.RaiseEventAsync("submission_delete", message);
    }
}
