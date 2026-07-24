// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ISubmissionProcessingService
{
    Submission GetSubmission(Guid submissionId);

    IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false);

    ValueTask<Submission> AddSubmissionAsync(Submission newSubmission);

    ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission);

    ValueTask DeleteAsync(Guid submissionId);

    ValueTask<IEnumerable<Result<Submission>>> AddOrUpdateSubmissionResult(IEnumerable<Submission> newSubmission);

    ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission);
}