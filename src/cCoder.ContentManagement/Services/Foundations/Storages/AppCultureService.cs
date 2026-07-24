// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppCultureService(IAppCultureBroker appCultureBroker, IAuthorizationBroker authorizationBroker) : IAppCultureService
{
    public IQueryable<AppCulture> GetAll(bool ignoreFilters = false) =>
        appCultureBroker.GetAllAppCultures(ignoreFilters: ignoreFilters);

    public AppCulture Get(int appId, string cultureId, bool ignoreFilters = false)
    {
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateCultureId(cultureId: cultureId, parameterName: "cultureId");

        return appCultureBroker.GetAllAppCultures(ignoreFilters: ignoreFilters)
            .FirstOrDefault(predicate: appCulture => appCulture.AppId == appId && appCulture.CultureId == cultureId);
    }

    public async ValueTask<AppCulture> AddAsync(AppCulture appCulture)
    {
        ValidateAppCulture(appCulture: appCulture, parameterName: "appCulture");
        authorizationBroker.Authorize(appId: appCulture.AppId, privilege: "AppCulture_create");
        AppCulture result = await appCultureBroker.AddAppCultureAsync(entity: CreateStorageAppCulture(appCulture: appCulture));
        appCulture.AppId = result.AppId;
        appCulture.CultureId = result.CultureId;
        return appCulture;
    }

    public async ValueTask DeleteAsync(AppCulture appCulture)
    {
        ValidateAppCulture(appCulture: appCulture, parameterName: "appCulture");
        authorizationBroker.Authorize(appId: appCulture.AppId, privilege: "AppCulture_delete");
        await appCultureBroker.DeleteAppCultureAsync(entity: CreateStorageAppCulture(appCulture: appCulture));
    }

    private static AppCulture CreateStorageAppCulture(AppCulture appCulture)
    {
        if (appCulture == null)
        {
            return null;
        }

        return new AppCulture
        {
            AppId = appCulture.AppId,
            CultureId = appCulture.CultureId
        };
    }
}