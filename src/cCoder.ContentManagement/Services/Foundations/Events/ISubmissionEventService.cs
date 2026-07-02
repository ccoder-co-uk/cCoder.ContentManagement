using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface ISubmissionEventService
{
    ValueTask RaiseSubmissionAddEventAsync(Submission entity);

    ValueTask RaiseSubmissionUpdateEventAsync(Submission entity);

    ValueTask RaiseSubmissionDeleteEventAsync(Submission entity);
}
