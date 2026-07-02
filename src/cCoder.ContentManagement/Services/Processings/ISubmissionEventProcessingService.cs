using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

public interface ISubmissionEventProcessingService
{
    ValueTask RaiseSubmissionAddEventAsync(Submission entity);

    ValueTask RaiseSubmissionUpdateEventAsync(Submission entity);

    ValueTask RaiseSubmissionDeleteEventAsync(Submission entity);
}
