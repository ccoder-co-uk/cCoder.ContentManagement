// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class SubmissionEventProcessingService(ISubmissionEventService eventService) : ISubmissionEventProcessingService
{
    public ValueTask RaiseSubmissionAddEventAsync(Submission submission) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionAddEventAsync(inputs: [submission]);
        ValidateSubmission(submission: submission, parameterName: "entity");

        return eventService.RaiseSubmissionAddEventAsync(entity: submission);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission submission) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionUpdateEventAsync(inputs: [submission]);
        ValidateSubmission(submission: submission, parameterName: "entity");

        return eventService.RaiseSubmissionUpdateEventAsync(entity: submission);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission submission) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionDeleteEventAsync(inputs: [submission]);
        ValidateSubmission(submission: submission, parameterName: "entity");

        return eventService.RaiseSubmissionDeleteEventAsync(entity: submission);

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