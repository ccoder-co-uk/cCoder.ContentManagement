using cCoder.Data.Models.CMS;
using EventLibrary;
using EventLibrary.Models;

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
