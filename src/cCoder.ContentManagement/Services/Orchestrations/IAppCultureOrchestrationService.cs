// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppCultureOrchestrationService
{
    IQueryable<AppCulture> GetAllAppCulture(bool ignoreFilters = false);

    ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture);

    ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<OperationResult<AppCulture>>> AddOrUpdateAppCultureResult(IEnumerable<AppCulture> newAppCulture);

    ValueTask DeleteAllAppCultureAsync(IEnumerable<AppCulture> deletedAppCulture);
}