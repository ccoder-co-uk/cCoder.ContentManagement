// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures;

public interface IAppManager
{
    App Get(int appManagerId, bool ignoreFilters = false);

    App GetByDomain(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAll(bool ignoreFilters = false);

    ValueTask<App> AddAsync(App newApp);

    ValueTask<App> UpdateAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);

    bool IsAdmin(int appId, string userName);

    IQueryable<User> GetUsers(int appId);

    ValueTask UpdatePageOrderAsync(int appId, App updatedApp);
}