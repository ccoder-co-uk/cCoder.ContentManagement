// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface ISubmissionService
{
    Submission GetSubmission(Guid submissionId, bool ignoreFilters = false);

    IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false);

    ValueTask<Submission> AddSubmissionAsync(Submission newSubmission);

    ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission);

    ValueTask DeleteAsync(Guid submissionId);
}