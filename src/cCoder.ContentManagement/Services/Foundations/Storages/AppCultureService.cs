// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppCultureService(IAppCultureBroker appCultureBroker) : IAppCultureService
{
    public IQueryable<AppCulture> GetAllAppCulture(bool ignoreFilters = false) =>
        TryCatch<IQueryable<AppCulture>>(operation: () =>
    {
        ValidateAllAppCultureOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? appCultureBroker.GetAllAppCulturesIgnoringFilters()
            : appCultureBroker.GetAllAppCultures();
    });

    public AppCulture GetAppCulture(int appId, string cultureId, bool ignoreFilters = false) =>
        TryCatch<AppCulture>(operation: () =>
    {
        ValidateAppCultureOnGet(inputs: [appId, cultureId, ignoreFilters]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidateCultureId(cultureId: cultureId, parameterName: "cultureId");

        return (ignoreFilters
            ? appCultureBroker.GetAllAppCulturesIgnoringFilters()
            : appCultureBroker.GetAllAppCultures())
            .FirstOrDefault(predicate: appCulture => appCulture.AppId == appId && appCulture.CultureId == cultureId);

    });

    public ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture) =>
        TryCatch<AppCulture>(operation: async () =>
    {
        ValidateAppCultureOnAdd(inputs: [newAppCulture]);
        ValidateAppCulture(appCulture: newAppCulture, parameterName: "appCulture");
        AppCulture result = await appCultureBroker.AddAppCultureAsync(newAppCulture: CreateStorageAppCulture(newAppCulture: newAppCulture));
        newAppCulture.AppId = result.AppId;
        newAppCulture.CultureId = result.CultureId;
        return newAppCulture;

    }, isValueTask: true);

    public ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture) =>
        TryCatch(operation: async () =>
    {
        ValidateAppCultureOnDelete(inputs: [deletedAppCulture]);
        ValidateAppCulture(appCulture: deletedAppCulture, parameterName: "appCulture");
        await appCultureBroker.DeleteAppCultureAsync(deletedAppCulture: CreateStorageAppCulture(newAppCulture: deletedAppCulture));

    }, isValueTask: true);

    private static AppCulture CreateStorageAppCulture(AppCulture newAppCulture)
    {
        if (newAppCulture == null)
        {
            return null;
        }

        return new AppCulture
        {
            AppId = newAppCulture.AppId,
            CultureId = newAppCulture.CultureId
        };
    }
}