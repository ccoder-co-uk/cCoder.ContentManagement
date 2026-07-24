// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IAppCultureBroker
{
    IQueryable<AppCulture> GetAllAppCultures(bool ignoreFilters);

    ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture);

    ValueTask<int> DeleteAppCultureAsync(AppCulture deletedAppCulture);

    ValueTask DeleteAllAppCulturesAsync(IEnumerable<AppCulture> deletedAppCulture);
}