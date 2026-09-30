// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppCultureOrchestrationService
{
    IQueryable<AppCulture> GetAllAppCultures(bool ignoreFilters = false);
    ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture);
    ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<OperationResult<AppCulture>>> AddOrUpdateAppCultureResult(IEnumerable<AppCulture> newAppCulture);
    ValueTask DeleteAllAppCultureAsync(IEnumerable<AppCulture> deletedAppCulture);
}