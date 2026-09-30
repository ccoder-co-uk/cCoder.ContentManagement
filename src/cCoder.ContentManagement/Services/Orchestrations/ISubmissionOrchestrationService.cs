// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ISubmissionOrchestrationService
{
    Submission GetSubmission(Guid submissionId);
    IQueryable<Submission> GetAllSubmissions(bool ignoreFilters = false);
    ValueTask<Submission> AddSubmissionAsync(Submission newSubmission);
    ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission);
    ValueTask DeleteAsync(Guid submissionId);
    ValueTask<IEnumerable<OperationResult<Submission>>> AddOrUpdateSubmissionResult(IEnumerable<Submission> newSubmission);
    ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission);
}