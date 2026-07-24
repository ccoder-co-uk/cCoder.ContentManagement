// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class SubmissionOrchestrationService(
    ISubmissionProcessingService processingService,
    ISubmissionEventProcessingService eventService) : ISubmissionOrchestrationService
{
    public Submission GetSubmission(Guid submissionId) =>
        processingService.GetSubmission(submissionId: ValidateId(submissionId: submissionId, parameterName: "id"));

    public IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false) =>
        processingService.GetAllSubmission(ignoreFilters: ignoreFilters);

    public async ValueTask<Submission> AddSubmissionAsync(Submission newSubmission)
    {
        ValidateSubmission(submission: newSubmission, parameterName: "entity");

        Submission result = await processingService.AddSubmissionAsync(newSubmission: newSubmission);
        await eventService.RaiseSubmissionAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission)
    {
        ValidateSubmission(submission: updatedSubmission, parameterName: "entity");

        Submission result = await processingService.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);
        await eventService.RaiseSubmissionUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid submissionId)
    {
        ValidateId(submissionId: submissionId, parameterName: "id");

        Submission entity = processingService.GetSubmission(submissionId: submissionId);
        await eventService.RaiseSubmissionDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(submissionId: submissionId);
    }

    public ValueTask<IEnumerable<Result<Submission>>> AddOrUpdateSubmissionResult(IEnumerable<Submission> newSubmission) =>
        processingService.AddOrUpdateSubmissionResult(newSubmission: ValidateSubmissions(submissions: newSubmission, parameterName: "items"));

    public ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission) =>
        processingService.DeleteAllSubmissionAsync(deletedSubmission: ValidateSubmissions(submissions: deletedSubmission, parameterName: "items"));

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