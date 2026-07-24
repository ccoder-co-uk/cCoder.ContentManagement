// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppCultureOrchestrationService
{
    IQueryable<AppCulture> GetAll(bool ignoreFilters = false);

    ValueTask<AppCulture> AddAsync(AppCulture entity);

    ValueTask DeleteAsync(AppCulture entity);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<AppCulture>>> AddOrUpdate(IEnumerable<AppCulture> items);

    ValueTask DeleteAllAsync(IEnumerable<AppCulture> items);
}