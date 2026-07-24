// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ICultureService
{
    Culture Get(string id, bool ignoreFilters = false);

    IQueryable<Culture> GetAll(bool ignoreFilters = false);

    ValueTask<Culture> AddAsync(Culture culture);

    ValueTask<Culture> UpdateAsync(Culture culture);

    ValueTask DeleteAsync(string id);
}