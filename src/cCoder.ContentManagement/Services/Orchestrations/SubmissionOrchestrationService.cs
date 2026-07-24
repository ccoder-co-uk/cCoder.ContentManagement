// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class SubmissionOrchestrationService(
    ISubmissionProcessingService processingService,
    ISubmissionEventProcessingService eventService) : ISubmissionOrchestrationService
{
    public Submission GetSubmission(Guid submissionId) =>
        TryCatch<Submission>(operation: () =>
    {
        ValidateSubmissionOnGet(inputs: [submissionId]);
        return processingService.GetSubmission(submissionId: ValidateId(submissionId: submissionId, parameterName: "id"));
    });

    public IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Submission>>(operation: () =>
    {
        ValidateAllSubmissionOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllSubmission(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Submission> AddSubmissionAsync(Submission newSubmission) =>
        TryCatch<Submission>(operation: async () =>
    {
        ValidateSubmissionOnAdd(inputs: [newSubmission]);
        ValidateSubmission(submission: newSubmission, parameterName: "entity");

        Submission result = await processingService.AddSubmissionAsync(newSubmission: newSubmission);
        await eventService.RaiseSubmissionAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission) =>
        TryCatch<Submission>(operation: async () =>
    {
        ValidateSubmissionOnUpdate(inputs: [updatedSubmission]);
        ValidateSubmission(submission: updatedSubmission, parameterName: "entity");

        Submission result = await processingService.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);
        await eventService.RaiseSubmissionUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid submissionId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [submissionId]);
        ValidateId(submissionId: submissionId, parameterName: "id");

        Submission entity = processingService.GetSubmission(submissionId: submissionId);
        await eventService.RaiseSubmissionDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(submissionId: submissionId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<Submission>>> AddOrUpdateSubmissionResult(IEnumerable<Submission> newSubmission) =>
        TryCatch<IEnumerable<Result<Submission>>>(operation: () =>
    {
        ValidateOrUpdateSubmissionResultOnAdd(inputs: [newSubmission]);
        return processingService.AddOrUpdateSubmissionResult(newSubmission: ValidateSubmissions(submissions: newSubmission, parameterName: "items"));
    }, isValueTask: true);

    public ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission) =>
        TryCatch(operation: () =>
    {
        ValidateAllSubmissionOnDelete(inputs: [deletedSubmission]);
        return processingService.DeleteAllSubmissionAsync(deletedSubmission: ValidateSubmissions(submissions: deletedSubmission, parameterName: "items"));
    }, isValueTask: true);

    private static Guid ValidateId(Guid submissionId, string parameterName)
    {
        if (submissionId == Guid.Empty)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return submissionId;
    }

    private static Submission ValidateSubmission(Submission submission, string parameterName)
    {
        if (submission == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return submission;
    }

    private static IEnumerable<Submission> ValidateSubmissions(IEnumerable<Submission> submissions, string parameterName)
    {
        if (submissions == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return submissions;
    }
}