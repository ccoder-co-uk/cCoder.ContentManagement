// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IAppManager
{
    App Get(int id, bool ignoreFilters = false);

    App GetByDomain(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAll(bool ignoreFilters = false);

    ValueTask<App> AddAsync(App app);

    ValueTask<App> UpdateAsync(App app);

    ValueTask DeleteAsync(int appId);
}