// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IAppCultureProcessingService
{
    IQueryable<AppCulture> GetAllAppCulture(bool ignoreFilters = false);

    ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture);

    ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture);

    ValueTask<IEnumerable<OperationResult<AppCulture>>> AddOrUpdateAppCultureResult(IEnumerable<AppCulture> newAppCulture);

    ValueTask DeleteAllAppCultureAsync(IEnumerable<AppCulture> deletedAppCulture);
}