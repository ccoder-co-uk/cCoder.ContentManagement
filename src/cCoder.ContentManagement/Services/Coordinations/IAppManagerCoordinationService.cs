// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal interface IAppManagerCoordinationService
{
    App GetApp(int appId);

    App GetByDomainApp(string domain, bool ignoreFilters = false);

    IQueryable<App> GetAllApp(bool ignoreFilters = false);

    bool IsAdminApp(int appId, string userName);

    App GetAppWithUsers(int appId);

    ValueTask UpdatePageOrderAppAsync(int appId, App updatedApp);
}