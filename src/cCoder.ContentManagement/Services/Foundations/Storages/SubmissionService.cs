// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class SubmissionService(ISubmissionBroker submissionBroker, IAuthorizationManager authorizationManager) : ISubmissionService
{
    public Submission GetSubmission(Guid submissionId, bool ignoreFilters = false) =>
        TryCatch<Submission>(operation: () =>
    {
        ValidateSubmissionOnGet(inputs: [submissionId, ignoreFilters]);
        ValidateId(submissionId: submissionId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllSubmission(ignoreFilters: true)
                .FirstOrDefault(predicate: (Submission i) => i.Id == submissionId);
        }

        Submission submission = ExecuteGetAllSubmission()
            .FirstOrDefault(predicate: (Submission i) => i.Id == submissionId);

        if (submission != null)
        {
            return submission;
        }

        Submission submission2 = ExecuteGetAllSubmission(ignoreFilters: true)
            .FirstOrDefault(predicate: (Submission i) => i.Id == submissionId);

        if (submission2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Submission>>(operation: () =>
    {
        ValidateAllSubmissionOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? submissionBroker.GetAllSubmissionsIgnoringFilters()
            : submissionBroker.GetAllSubmissions();
    });

    public ValueTask<Submission> AddSubmissionAsync(Submission newSubmission) =>
        TryCatch<Submission>(operation: async () =>
    {
        ValidateSubmissionOnAdd(inputs: [newSubmission]);
        ValidateSubmission(submission: newSubmission, parameterName: "submission");
        authorizationManager.Authorize(appId: newSubmission.AppId, privilege: "Submission_create");
        Submission storageSubmission = CreateStorageSubmission(newSubmission: newSubmission);
        storageSubmission.Id = ((newSubmission.Id == Guid.Empty) ? Guid.NewGuid() : newSubmission.Id);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storageSubmission.CreatedOn = DateTimeOffset.UtcNow);
        storageSubmission.CreatedBy = currentUserId;
        storageSubmission.LastUpdatedOn = now;
        storageSubmission.LastUpdatedBy = currentUserId;
        Submission result = await submissionBroker.AddSubmissionAsync(newSubmission: storageSubmission);
        newSubmission.Id = result.Id;
        newSubmission.AppId = result.AppId;
        newSubmission.CreatedBy = result.CreatedBy;
        newSubmission.LastUpdatedBy = result.LastUpdatedBy;
        newSubmission.CreatedOn = result.CreatedOn;
        newSubmission.LastUpdatedOn = result.LastUpdatedOn;
        newSubmission.SourceComponent = result.SourceComponent;
        newSubmission.State = result.State;
        newSubmission.DataJson = result.DataJson;
        return newSubmission;

    }, isValueTask: true);

    public ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission) =>
        TryCatch<Submission>(operation: async () =>
    {
        ValidateSubmissionOnUpdate(inputs: [updatedSubmission]);
        ValidateSubmission(submission: updatedSubmission, parameterName: "submission");
        authorizationManager.Authorize(appId: updatedSubmission.AppId, privilege: "Submission_update");
        Submission updateSubmission = CreateStorageSubmission(newSubmission: updatedSubmission);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateSubmission.LastUpdatedOn = now;
        updateSubmission.LastUpdatedBy = currentUserId;
        Submission result = await submissionBroker.UpdateSubmissionAsync(updatedSubmission: updateSubmission);
        updatedSubmission.Id = result.Id;
        updatedSubmission.AppId = result.AppId;
        updatedSubmission.CreatedBy = result.CreatedBy;
        updatedSubmission.LastUpdatedBy = result.LastUpdatedBy;
        updatedSubmission.CreatedOn = result.CreatedOn;
        updatedSubmission.LastUpdatedOn = result.LastUpdatedOn;
        updatedSubmission.SourceComponent = result.SourceComponent;
        updatedSubmission.State = result.State;
        updatedSubmission.DataJson = result.DataJson;
        return updatedSubmission;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid submissionId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [submissionId]);
        ValidateId(submissionId: submissionId, parameterName: "id");
        Submission submission = ExecuteGetSubmission(submissionId: submissionId);
        authorizationManager.Authorize(appId: submission.AppId, privilege: "Submission_delete");
        await submissionBroker.DeleteSubmissionAsync(deletedSubmission: CreateStorageSubmission(newSubmission: submission));

    }, isValueTask: true);

    private static Submission CreateStorageSubmission(Submission newSubmission)
    {
        if (newSubmission == null)
        {
            return null;
        }

        return new Submission
        {
            Id = newSubmission.Id,
            AppId = newSubmission.AppId,
            CreatedBy = newSubmission.CreatedBy,
            LastUpdatedBy = newSubmission.LastUpdatedBy,
            CreatedOn = newSubmission.CreatedOn,
            LastUpdatedOn = newSubmission.LastUpdatedOn,
            SourceComponent = newSubmission.SourceComponent,
            State = newSubmission.State,
            DataJson = newSubmission.DataJson
        };
    }

    private IQueryable<Submission> ExecuteGetAllSubmission(bool ignoreFilters = false) =>
        (ignoreFilters
            ? submissionBroker.GetAllSubmissionsIgnoringFilters()
            : submissionBroker.GetAllSubmissions());

    private Submission ExecuteGetSubmission(Guid submissionId, bool ignoreFilters = false)
    {
        ValidateId(submissionId: submissionId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllSubmission(ignoreFilters: true)
                .FirstOrDefault(predicate: (Submission i) => i.Id == submissionId);
        }

        Submission submission = ExecuteGetAllSubmission()
            .FirstOrDefault(predicate: (Submission i) => i.Id == submissionId);

        if (submission != null)
        {
            return submission;
        }

        Submission submission2 = ExecuteGetAllSubmission(ignoreFilters: true)
            .FirstOrDefault(predicate: (Submission i) => i.Id == submissionId);

        if (submission2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}