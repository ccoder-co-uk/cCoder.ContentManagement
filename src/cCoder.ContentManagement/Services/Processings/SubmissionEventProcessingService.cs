// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class SubmissionEventProcessingService(ISubmissionEventService eventService) : ISubmissionEventProcessingService
{
    public ValueTask RaiseSubmissionAddEventAsync(Submission entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionAddEventAsync(inputs: [entity]);
        ValidateSubmission(submission: entity, parameterName: "entity");

        return eventService.RaiseSubmissionAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionUpdateEventAsync(inputs: [entity]);
        ValidateSubmission(submission: entity, parameterName: "entity");

        return eventService.RaiseSubmissionUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionDeleteEventAsync(inputs: [entity]);
        ValidateSubmission(submission: entity, parameterName: "entity");

        return eventService.RaiseSubmissionDeleteEventAsync(entity: entity);

    }, isValueTask: true);

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