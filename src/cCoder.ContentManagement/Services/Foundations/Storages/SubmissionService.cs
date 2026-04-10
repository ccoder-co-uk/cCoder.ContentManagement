using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class SubmissionService(ISubmissionBroker submissionBroker, IAuthorizationBroker authorizationBroker) : ISubmissionService
{
    public Submission Get(Guid id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Submission i) => i.Id == id);
        }

        Submission submission = GetAll().FirstOrDefault((Submission i) => i.Id == id);
        if (submission != null)
        {
            return submission;
        }
        Submission submission2 = GetAll(ignoreFilters: true).FirstOrDefault((Submission i) => i.Id == id);
        if (submission2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Submission> GetAll(bool ignoreFilters = false)
    {
        return submissionBroker.GetAllSubmissions(ignoreFilters);
    }

    public async ValueTask<Submission> AddAsync(Submission submission)
    {
        ValidateSubmission(submission, "submission");
        authorizationBroker.Authorize(submission.AppId, "Submission_create");
        Submission newSubmission = CreateStorageSubmission(submission);
        newSubmission.Id = ((submission.Id == Guid.Empty) ? Guid.NewGuid() : submission.Id);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newSubmission.CreatedOn = DateTimeOffset.UtcNow);
        newSubmission.CreatedBy = currentUserId;
        newSubmission.LastUpdatedOn = now;
        newSubmission.LastUpdatedBy = currentUserId;
        Submission result = await submissionBroker.AddSubmissionAsync(newSubmission);
        result.App = submission.App;
        return result;
    }

    public async ValueTask<Submission> UpdateAsync(Submission submission)
    {
        ValidateSubmission(submission, "submission");
        authorizationBroker.Authorize(submission.AppId, "Submission_update");
        Submission updateSubmission = CreateStorageSubmission(submission);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateSubmission.LastUpdatedOn = now;
        updateSubmission.LastUpdatedBy = currentUserId;
        Submission result = await submissionBroker.UpdateSubmissionAsync(updateSubmission);
        result.App = submission.App;
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id, "id");
        Submission submission = Get(id);
        authorizationBroker.Authorize(submission.AppId, "Submission_delete");
        await submissionBroker.DeleteSubmissionAsync(CreateStorageSubmission(submission));
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
