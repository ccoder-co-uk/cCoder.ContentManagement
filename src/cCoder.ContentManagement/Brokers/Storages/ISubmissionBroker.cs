// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ISubmissionBroker
{
    IQueryable<Submission> GetAllSubmissions();

    IQueryable<Submission> GetAllSubmissionsIgnoringFilters();

    ValueTask<Submission> AddSubmissionAsync(Submission newSubmission);

    ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission);

    ValueTask<int> DeleteSubmissionAsync(Submission deletedSubmission);

    ValueTask DeleteAllSubmissionsAsync(IEnumerable<Submission> deletedSubmission);
}