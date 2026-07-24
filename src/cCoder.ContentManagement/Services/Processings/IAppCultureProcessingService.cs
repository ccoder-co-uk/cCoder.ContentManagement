// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IAppCultureProcessingService
{
    IQueryable<AppCulture> GetAll(bool ignoreFilters = false);

    ValueTask<AppCulture> AddAsync(AppCulture entity);

    ValueTask DeleteAsync(AppCulture entity);

    ValueTask<IEnumerable<Result<AppCulture>>> AddOrUpdate(IEnumerable<AppCulture> items);

    ValueTask DeleteAllAsync(IEnumerable<AppCulture> items);
}