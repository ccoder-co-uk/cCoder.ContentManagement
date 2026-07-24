// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ISubmissionService
{
    Submission Get(Guid id, bool ignoreFilters = false);

    IQueryable<Submission> GetAll(bool ignoreFilters = false);

    ValueTask<Submission> AddAsync(Submission submission);

    ValueTask<Submission> UpdateAsync(Submission submission);

    ValueTask DeleteAsync(Guid id);
}