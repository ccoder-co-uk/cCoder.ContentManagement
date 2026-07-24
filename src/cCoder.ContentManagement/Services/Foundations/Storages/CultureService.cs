// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CultureService(
    ICultureBroker cultureBroker,
    IAppCultureBroker appCultureBroker,
    IAuthorizationBroker authorizationBroker) : ICultureService
{
    public Culture Get(string id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (Culture i) => i.Id == id);
        }

        Culture culture = GetAll()
            .FirstOrDefault(predicate: (Culture i) => i.Id == id);

        if ((object)culture != null)
        {
            return culture;
        }

        Culture culture2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (Culture i) => i.Id == id);

        if ((object)culture2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Culture> GetAll(bool ignoreFilters = false) =>
        cultureBroker.GetAllCultures(ignoreFilters: ignoreFilters);

    public async ValueTask<Culture> AddAsync(Culture culture)
    {
        ValidateCulture(culture: culture, parameterName: "culture");
        authorizationBroker.Authorize(appId: GetAppId(cultureId: culture.Id), privilege: "Culture_create");
        Culture result = await cultureBroker.AddCultureAsync(entity: CreateStorageCulture(culture: culture));
        culture.Id = result.Id;
        culture.Name = result.Name;
        return culture;
    }

    public async ValueTask<Culture> UpdateAsync(Culture culture)
    {
        ValidateCulture(culture: culture, parameterName: "culture");
        authorizationBroker.Authorize(appId: GetAppId(cultureId: culture.Id), privilege: "Culture_update");
        Culture result = await cultureBroker.UpdateCultureAsync(entity: CreateStorageCulture(culture: culture));
        culture.Id = result.Id;
        culture.Name = result.Name;
        return culture;
    }

    public async ValueTask DeleteAsync(string id)
    {
        ValidateId(id: id, parameterName: "id");
        Culture culture = Get(id: id);
        authorizationBroker.Authorize(appId: GetAppId(cultureId: culture.Id), privilege: "Culture_delete");
        await cultureBroker.DeleteCultureAsync(entity: CreateStorageCulture(culture: culture));
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
            .Where(predicate: appCulture => appCulture.CultureId == cultureId)
            .Select(selector: appCulture => (int?)appCulture.AppId)
            .FirstOrDefault();
    }
}