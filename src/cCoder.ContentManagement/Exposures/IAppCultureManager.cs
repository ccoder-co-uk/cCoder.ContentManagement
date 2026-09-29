// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IAppCultureManager
{
    IQueryable<AppCulture> GetAllAppCulture(bool ignoreFilters = false);

    ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture);

    ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<OperationResult<AppCulture>>> AddOrUpdateAppCultureResult(IEnumerable<AppCulture> newAppCulture);

    ValueTask DeleteAllAppCultureAsync(IEnumerable<AppCulture> deletedAppCulture);
}