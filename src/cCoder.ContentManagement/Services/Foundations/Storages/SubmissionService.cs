// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class SubmissionService(ISubmissionBroker submissionBroker, IAuthorizationBroker authorizationBroker) : ISubmissionService
{
    public Submission Get(Guid id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (Submission i) => i.Id == id);
        }

        Submission submission = GetAll()
            .FirstOrDefault(predicate: (Submission i) => i.Id == id);

        if (submission != null)
        {
            return submission;
        }

        Submission submission2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (Submission i) => i.Id == id);

        if (submission2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Submission> GetAll(bool ignoreFilters = false) =>
        submissionBroker.GetAllSubmissions(ignoreFilters: ignoreFilters);

    public async ValueTask<Submission> AddAsync(Submission submission)
    {
        ValidateSubmission(submission: submission, parameterName: "submission");
        authorizationBroker.Authorize(appId: submission.AppId, privilege: "Submission_create");
        Submission newSubmission = CreateStorageSubmission(submission: submission);
        newSubmission.Id = ((submission.Id == Guid.Empty) ? Guid.NewGuid() : submission.Id);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newSubmission.CreatedOn = DateTimeOffset.UtcNow);
        newSubmission.CreatedBy = currentUserId;
        newSubmission.LastUpdatedOn = now;
        newSubmission.LastUpdatedBy = currentUserId;
        Submission result = await submissionBroker.AddSubmissionAsync(entity: newSubmission);
        submission.Id = result.Id;
        submission.AppId = result.AppId;
        submission.CreatedBy = result.CreatedBy;
        submission.LastUpdatedBy = result.LastUpdatedBy;
        submission.CreatedOn = result.CreatedOn;
        submission.LastUpdatedOn = result.LastUpdatedOn;
        submission.SourceComponent = result.SourceComponent;
        submission.State = result.State;
        submission.DataJson = result.DataJson;
        return submission;
    }

    public async ValueTask<Submission> UpdateAsync(Submission submission)
    {
        ValidateSubmission(submission: submission, parameterName: "submission");
        authorizationBroker.Authorize(appId: submission.AppId, privilege: "Submission_update");
        Submission updateSubmission = CreateStorageSubmission(submission: submission);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateSubmission.LastUpdatedOn = now;
        updateSubmission.LastUpdatedBy = currentUserId;
        Submission result = await submissionBroker.UpdateSubmissionAsync(entity: updateSubmission);
        submission.Id = result.Id;
        submission.AppId = result.AppId;
        submission.CreatedBy = result.CreatedBy;
        submission.LastUpdatedBy = result.LastUpdatedBy;
        submission.CreatedOn = result.CreatedOn;
        submission.LastUpdatedOn = result.LastUpdatedOn;
        submission.SourceComponent = result.SourceComponent;
        submission.State = result.State;
        submission.DataJson = result.DataJson;
        return submission;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id: id, parameterName: "id");
        Submission submission = Get(id: id);
        authorizationBroker.Authorize(appId: submission.AppId, privilege: "Submission_delete");
        await submissionBroker.DeleteSubmissionAsync(entity: CreateStorageSubmission(submission: submission));
    }

    private static Submission CreateStorageSubmission(Submission submission)
    {
        if (submission == null)
        {
            return null;
        }

        return new Submission
        {
            Id = submission.Id,
            AppId = submission.AppId,
            CreatedBy = submission.CreatedBy,
            LastUpdatedBy = submission.LastUpdatedBy,
            CreatedOn = submission.CreatedOn,
            LastUpdatedOn = submission.LastUpdatedOn,
            SourceComponent = submission.SourceComponent,
            State = submission.State,
            DataJson = submission.DataJson
        };
    }
}