// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class SubmissionEventProcessingService(ISubmissionEventService eventService)
    : ISubmissionEventProcessingService
{
    public ValueTask RaiseSubmissionAddEventAsync(Submission submission, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionAddEventAsync(inputs: [submission, userId]);
        ValidateSubmission(submission: submission, parameterName: "entity");


        return eventService.RaiseSubmissionAddEventAsync(
            entity: submission,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission submission, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionUpdateEventAsync(inputs: [submission, userId]);
        ValidateSubmission(submission: submission, parameterName: "entity");


        return eventService.RaiseSubmissionUpdateEventAsync(
            entity: submission,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission submission, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseSubmissionDeleteEventAsync(inputs: [submission, userId]);
        ValidateSubmission(submission: submission, parameterName: "entity");


        return eventService.RaiseSubmissionDeleteEventAsync(
            entity: submission,
            userId: userId);

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