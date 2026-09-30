// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IAppProcessingService
{
    App GetApp(int appId);

    ValueTask<App> GetAppForRenderAsync(int appId);

    App GetAppForDelete(int appId);

    string GetDomain(int appId, bool ignoreFilters = false);

    App GetByDomainApp(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAllApps(bool ignoreFilters = false);

    App PrepareNewApp(App app, bool isFirstApp);

    void StampAppChildren(App app);

    ValueTask PersistNewAppRolesAsync(App app);

    ValueTask<App> AddAppAsync(App newApp);

    ValueTask<App> UpdateAppAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);

    ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp);
}