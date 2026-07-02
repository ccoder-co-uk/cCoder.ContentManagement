using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class SubmissionEventProcessingService(ISubmissionEventService eventService) : ISubmissionEventProcessingService
{
    public ValueTask RaiseSubmissionAddEventAsync(Submission entity)
    {
        ValidateSubmission(entity, "entity");

        return eventService.RaiseSubmissionAddEventAsync(entity);
    }

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission entity)
    {
        ValidateSubmission(entity, "entity");

        return eventService.RaiseSubmissionUpdateEventAsync(entity);
    }

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission entity)
    {
        ValidateSubmission(entity, "entity");

        return eventService.RaiseSubmissionDeleteEventAsync(entity);
    }

    private static void ValidateSubmission(Submission submission, string parameterName) =>
        ThrowIf(submission == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
