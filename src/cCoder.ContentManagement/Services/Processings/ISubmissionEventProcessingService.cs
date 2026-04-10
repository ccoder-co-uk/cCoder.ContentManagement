using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Services.Processings;

public interface ISubmissionEventProcessingService
{
    ValueTask RaiseSubmissionAddEventAsync(Submission entity);

    ValueTask RaiseSubmissionUpdateEventAsync(Submission entity);

    ValueTask RaiseSubmissionDeleteEventAsync(Submission entity);
}
