// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class SubmissionManager(ISubmissionOrchestrationService service) : ISubmissionManager
{
    public Submission GetSubmission(Guid submissionId) =>
        service.GetSubmission(submissionId: submissionId);

    public IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false) =>
        service.GetAllSubmission(ignoreFilters: ignoreFilters);

    public ValueTask<Submission> AddSubmissionAsync(Submission newSubmission) =>
        service.AddSubmissionAsync(newSubmission: newSubmission);

    public ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission) =>
        service.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);

    public ValueTask DeleteAsync(Guid submissionId) =>
        service.DeleteAsync(submissionId: submissionId);

    public ValueTask<IEnumerable<OperationResult<Submission>>> AddOrUpdateSubmissionResult(
        IEnumerable<Submission> newSubmission) =>
        service.AddOrUpdateSubmissionResult(newSubmission: newSubmission);

    public ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission) =>
        service.DeleteAllSubmissionAsync(deletedSubmission: deletedSubmission);
}