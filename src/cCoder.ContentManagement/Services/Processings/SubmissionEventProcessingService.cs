using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Services.Processings;

internal class SubmissionEventProcessingService(ISubmissionEventService eventService) : ISubmissionEventProcessingService
{
    public ValueTask RaiseSubmissionAddEventAsync(Submission entity)
    {
        return eventService.RaiseSubmissionAddEventAsync(ValidateSubmission(entity, "entity"));
    }

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission entity)
    {
        return eventService.RaiseSubmissionUpdateEventAsync(ValidateSubmission(entity, "entity"));
    }

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission entity)
    {
        return eventService.RaiseSubmissionDeleteEventAsync(ValidateSubmission(entity, "entity"));
    }

    private static Submission ValidateSubmission(Submission submission, string parameterName)
    {
        if (submission == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return submission;
    }
}
