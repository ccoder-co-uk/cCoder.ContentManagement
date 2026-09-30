// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class AppCultureManager(IAppCultureOrchestrationService service) : IAppCultureManager
{
    public IQueryable<AppCulture> GetAllAppCultures(bool ignoreFilters = false) =>
        service.GetAllAppCultures(ignoreFilters: ignoreFilters);

    public ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture) =>
        service.AddAppCultureAsync(newAppCulture: newAppCulture);

    public ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture) =>
        service.DeleteAppCultureAsync(deletedAppCulture: deletedAppCulture);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<OperationResult<AppCulture>>> AddOrUpdateAppCultureResult(
        IEnumerable<AppCulture> newAppCulture) =>
        service.AddOrUpdateAppCultureResult(newAppCulture: newAppCulture);

    public ValueTask DeleteAllAppCultureAsync(IEnumerable<AppCulture> deletedAppCulture) =>
        service.DeleteAllAppCultureAsync(deletedAppCulture: deletedAppCulture);
}