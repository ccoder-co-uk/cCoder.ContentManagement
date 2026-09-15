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
    ISubmissionEventProcessingService eventService,
    IAuthorizationProcessingService authorizationProcessingService)
        : ISubmissionOrchestrationService
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
        Authorize(appId: newSubmission.AppId, privilege: "Submission_create");
        StampForAdd(submission: newSubmission);

        Submission result = await processingService.AddSubmissionAsync(newSubmission: newSubmission);

        await eventService.RaiseSubmissionAddEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;

    }, isValueTask: true);

    public ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission) =>
        TryCatch<Submission>(operation: async () =>
    {
        ValidateSubmissionOnUpdate(inputs: [updatedSubmission]);
        ValidateSubmission(submission: updatedSubmission, parameterName: "entity");
        Authorize(appId: updatedSubmission.AppId, privilege: "Submission_update");
        StampForUpdate(submission: updatedSubmission);

        Submission result = await processingService.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);

        await eventService.RaiseSubmissionUpdateEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid submissionId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [submissionId]);
        ValidateId(submissionId: submissionId, parameterName: "id");

        Submission entity = processingService.GetSubmission(submissionId: submissionId);
        Authorize(appId: entity.AppId, privilege: "Submission_delete");

        await eventService.RaiseSubmissionDeleteEventAsync(
            entity: entity,
            userId: authorizationProcessingService.GetCurrentUserId());

        await processingService.DeleteAsync(submissionId: submissionId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Submission>>> AddOrUpdateSubmissionResult(IEnumerable<Submission> newSubmission) =>
        TryCatch<IEnumerable<OperationResult<Submission>>>(operation: async () =>
    {
        ValidateOrUpdateSubmissionResultOnAdd(inputs: [newSubmission]);

        Submission[] submissions = ValidateSubmissions(
            submissions: newSubmission,
            parameterName: "items")
            .ToArray();

        List<OperationResult<Submission>> results = new();

        foreach (Submission submission in submissions)
        {
            try
            {
                bool isNew = submission.Id == Guid.Empty;

                Submission result = isNew
                    ? await ExecuteAddSubmissionAsync(newSubmission: submission)
                    : await ExecuteUpdateSubmissionAsync(updatedSubmission: submission);

                results.Add(item: new OperationResult<Submission>
                {
                    Success = true,
                    Item = result,
                    Message = isNew
                        ? "Added Successfully"
                        : "Updated Successfully"
                });
            }
            catch (Exception exception)
            {
                results.Add(item: new OperationResult<Submission>
                {
                    Success = false,
                    Item = submission,
                    Message = exception.Message
                });
            }
        }

        return results;
    }, isValueTask: true);

    public ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission) =>
        TryCatch(operation: async () =>
    {
        ValidateAllSubmissionOnDelete(inputs: [deletedSubmission]);

        Submission[] submissions = ValidateSubmissions(
            submissions: deletedSubmission,
            parameterName: "items")
            .ToArray();

        foreach (Submission submission in submissions)
        {
            await ExecuteDeleteSubmissionAsync(submission: submission);
        }
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

    private void Authorize(int? appId, string privilege) =>
        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = appId,
                    Privilege = privilege
                }
            });

    private async ValueTask<Submission> ExecuteAddSubmissionAsync(
        Submission newSubmission)
    {
        Authorize(appId: newSubmission.AppId, privilege: "Submission_create");
        StampForAdd(submission: newSubmission);

        Submission result = await processingService.AddSubmissionAsync(
            newSubmission: newSubmission);

        await eventService.RaiseSubmissionAddEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;
    }

    private async ValueTask ExecuteDeleteSubmissionAsync(Submission submission)
    {
        Authorize(appId: submission.AppId, privilege: "Submission_delete");

        await eventService.RaiseSubmissionDeleteEventAsync(
            entity: submission,
            userId: authorizationProcessingService.GetCurrentUserId());

        await processingService.DeleteAsync(submissionId: submission.Id);
    }

    private async ValueTask<Submission> ExecuteUpdateSubmissionAsync(
        Submission updatedSubmission)
    {
        Authorize(appId: updatedSubmission.AppId, privilege: "Submission_update");
        StampForUpdate(submission: updatedSubmission);

        Submission result = await processingService.UpdateSubmissionAsync(
            updatedSubmission: updatedSubmission);

        await eventService.RaiseSubmissionUpdateEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;
    }

    private void StampForAdd(Submission submission)
    {
        string userId = authorizationProcessingService.GetCurrentUserId();
        submission.CreatedBy = userId;
        submission.LastUpdatedBy = userId;
    }

    private void StampForUpdate(Submission submission) =>
        submission.LastUpdatedBy = authorizationProcessingService.GetCurrentUserId();
}