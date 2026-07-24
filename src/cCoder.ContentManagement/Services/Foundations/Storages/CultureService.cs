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
    public Culture GetCulture(string cultureId, bool ignoreFilters = false)
    {
        ValidateId(cultureId: cultureId, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAllCulture(ignoreFilters: true)
                .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);
        }

        Culture culture = GetAllCulture()
            .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);

        if ((object)culture != null)
        {
            return culture;
        }

        Culture culture2 = GetAllCulture(ignoreFilters: true)
            .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);

        if ((object)culture2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Culture> GetAllCulture(bool ignoreFilters = false) =>
        cultureBroker.GetAllCultures(ignoreFilters: ignoreFilters);

    public async ValueTask<Culture> AddCultureAsync(Culture newCulture)
    {
        ValidateCulture(culture: newCulture, parameterName: "culture");
        authorizationBroker.Authorize(appId: GetAppId(cultureId: newCulture.Id), privilege: "Culture_create");
        Culture result = await cultureBroker.AddCultureAsync(newCulture: CreateStorageCulture(newCulture: newCulture));
        newCulture.Id = result.Id;
        newCulture.Name = result.Name;
        return newCulture;
    }

    public async ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture)
    {
        ValidateCulture(culture: updatedCulture, parameterName: "culture");
        authorizationBroker.Authorize(appId: GetAppId(cultureId: updatedCulture.Id), privilege: "Culture_update");
        Culture result = await cultureBroker.UpdateCultureAsync(updatedCulture: CreateStorageCulture(newCulture: updatedCulture));
        updatedCulture.Id = result.Id;
        updatedCulture.Name = result.Name;
        return updatedCulture;
    }

    public async ValueTask DeleteAsync(string cultureId)
    {
        ValidateId(cultureId: cultureId, parameterName: "id");
        Culture culture = GetCulture(cultureId: cultureId);
        authorizationBroker.Authorize(appId: GetAppId(cultureId: culture.Id), privilege: "Culture_delete");
        await cultureBroker.DeleteCultureAsync(deletedCulture: CreateStorageCulture(newCulture: culture));
    }

    private static Culture CreateStorageCulture(Culture newCulture)
    {
        if (newCulture == null)
        {
            return null;
        }

        return new Culture
        {
            Id = newCulture.Id,
            Name = newCulture.Name
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