// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IAppCultureService
{
    IQueryable<AppCulture> GetAllAppCulture(bool ignoreFilters = false);

    AppCulture GetAppCulture(int appId, string cultureId, bool ignoreFilters = false);

    ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture);

    ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture);
}