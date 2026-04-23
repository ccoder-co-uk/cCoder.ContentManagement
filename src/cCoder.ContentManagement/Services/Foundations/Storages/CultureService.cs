using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CultureService(
    ICultureBroker cultureBroker,
    IAppCultureBroker appCultureBroker,
    IAuthorizationBroker authorizationBroker) : ICultureService
{
    public Culture Get(string id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Culture i) => i.Id == id);
        }

        Culture culture = GetAll().FirstOrDefault((Culture i) => i.Id == id);
        if ((object)culture != null)
        {
            return culture;
        }
        Culture culture2 = GetAll(ignoreFilters: true).FirstOrDefault((Culture i) => i.Id == id);
        if ((object)culture2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Culture> GetAll(bool ignoreFilters = false)
    {
        return cultureBroker.GetAllCultures(ignoreFilters);
    }

    public async ValueTask<Culture> AddAsync(Culture culture)
    {
        ValidateCulture(culture, "culture");
        authorizationBroker.Authorize(GetAppId(culture.Id), "Culture_create");
        Culture result = await cultureBroker.AddCultureAsync(CreateStorageCulture(culture));
        culture.Id = result.Id;
        culture.Name = result.Name;
        return culture;
    }

    public async ValueTask<Culture> UpdateAsync(Culture culture)
    {
        ValidateCulture(culture, "culture");
        authorizationBroker.Authorize(GetAppId(culture.Id), "Culture_update");
        Culture result = await cultureBroker.UpdateCultureAsync(CreateStorageCulture(culture));
        culture.Id = result.Id;
        culture.Name = result.Name;
        return culture;
    }

    public async ValueTask DeleteAsync(string id)
    {
        ValidateId(id, "id");
        Culture culture = Get(id);
        authorizationBroker.Authorize(GetAppId(culture.Id), "Culture_delete");
        await cultureBroker.DeleteCultureAsync(CreateStorageCulture(culture));
    }

    private static Culture CreateStorageCulture(Culture culture)
    {
        if (culture == null)
        {
            return null;
        }

        return new Culture
        {
            Id = culture.Id,
            Name = culture.Name
        };
    }

    private int? GetAppId(string cultureId)
    {
        return appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(appCulture => appCulture.CultureId == cultureId)
            .Select(appCulture => (int?)appCulture.AppId)
            .FirstOrDefault();
    }
}
