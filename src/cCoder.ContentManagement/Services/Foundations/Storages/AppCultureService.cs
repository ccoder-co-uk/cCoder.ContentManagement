using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppCultureService(IAppCultureBroker appCultureBroker, IAuthorizationBroker authorizationBroker) : IAppCultureService
{
    public IQueryable<AppCulture> GetAll(bool ignoreFilters = false)
    {
        return appCultureBroker.GetAllAppCultures(ignoreFilters);
    }

    public AppCulture Get(int appId, string cultureId, bool ignoreFilters = false)
    {
        ValidateAppId(appId, "appId");
        ValidateCultureId(cultureId, "cultureId");
        return appCultureBroker.GetAllAppCultures(ignoreFilters)
            .FirstOrDefault(appCulture => appCulture.AppId == appId && appCulture.CultureId == cultureId);
    }

    public async ValueTask<AppCulture> AddAsync(AppCulture appCulture)
    {
        ValidateAppCulture(appCulture, "appCulture");
        authorizationBroker.Authorize(appCulture.AppId, "AppCulture_create");
        AppCulture result = await appCultureBroker.AddAppCultureAsync(CreateStorageAppCulture(appCulture));
        result.App = appCulture.App;
        result.Culture = appCulture.Culture;
        return result;
    }

    public async ValueTask DeleteAsync(AppCulture appCulture)
    {
        ValidateAppCulture(appCulture, "appCulture");
        authorizationBroker.Authorize(appCulture.AppId, "AppCulture_delete");
        await appCultureBroker.DeleteAppCultureAsync(CreateStorageAppCulture(appCulture));
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
