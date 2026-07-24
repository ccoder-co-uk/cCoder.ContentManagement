// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class SubmissionEventProcessingService(ISubmissionEventService eventService) : ISubmissionEventProcessingService
{
    public ValueTask RaiseSubmissionAddEventAsync(Submission entity)
    {
        ValidateSubmission(submission: entity, parameterName: "entity");

        return eventService.RaiseSubmissionAddEventAsync(entity: entity);
    }

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission entity)
    {
        ValidateSubmission(submission: entity, parameterName: "entity");

        return eventService.RaiseSubmissionUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission entity)
    {
        ValidateSubmission(submission: entity, parameterName: "entity");

        return eventService.RaiseSubmissionDeleteEventAsync(entity: entity);
    }

    private static void ValidateSubmission(Submission submission, string parameterName) =>
        ThrowIf(condition: submission == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}