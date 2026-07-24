// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IAppManager
{
    App Get(int appManagerId, bool ignoreFilters = false);

    App GetByDomain(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAll(bool ignoreFilters = false);

    ValueTask<App> AddAsync(App newApp);

    ValueTask<App> UpdateAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);
}