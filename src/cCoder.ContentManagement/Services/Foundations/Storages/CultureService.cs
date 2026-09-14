// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CultureService(ICultureBroker cultureBroker) : ICultureService
{
    public Culture GetCulture(string cultureId, bool ignoreFilters = false) =>
        TryCatch<Culture>(operation: () =>
    {
        ValidateCultureOnGet(inputs: [cultureId, ignoreFilters]);
        ValidateId(cultureId: cultureId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllCulture(ignoreFilters: true)
                .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);
        }

        Culture culture = ExecuteGetAllCulture()
            .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);

        if ((object)culture != null)
        {
            return culture;
        }

        Culture culture2 = ExecuteGetAllCulture(ignoreFilters: true)
            .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);

        if ((object)culture2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Culture> GetAllCulture(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Culture>>(operation: () =>
    {
        ValidateAllCultureOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? cultureBroker.GetAllCulturesIgnoringFilters()
            : cultureBroker.GetAllCultures();
    });

    public int? GetOwningAppId(string cultureId) =>
        TryCatch<int?>(operation: () =>
    {
        ValidateOwningAppIdOnGet(inputs: [cultureId]);
        ValidateId(cultureId: cultureId, parameterName: "id");
        return cultureBroker.GetOwningAppId(cultureId: cultureId);
    });

    public ValueTask<Culture> AddCultureAsync(Culture newCulture) =>
        TryCatch<Culture>(operation: async () =>
    {
        ValidateCultureOnAdd(inputs: [newCulture]);
        ValidateCulture(culture: newCulture, parameterName: "culture");
        Culture result = await cultureBroker.AddCultureAsync(newCulture: CreateStorageCulture(newCulture: newCulture));
        newCulture.Id = result.Id;
        newCulture.Name = result.Name;
        return newCulture;

    }, isValueTask: true);

    public ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture) =>
        TryCatch<Culture>(operation: async () =>
    {
        ValidateCultureOnUpdate(inputs: [updatedCulture]);
        ValidateCulture(culture: updatedCulture, parameterName: "culture");
        Culture result = await cultureBroker.UpdateCultureAsync(updatedCulture: CreateStorageCulture(newCulture: updatedCulture));
        updatedCulture.Id = result.Id;
        updatedCulture.Name = result.Name;
        return updatedCulture;

    }, isValueTask: true);

    public ValueTask DeleteAsync(string cultureId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [cultureId]);
        ValidateId(cultureId: cultureId, parameterName: "id");
        Culture culture = ExecuteGetCulture(cultureId: cultureId);
        await cultureBroker.DeleteCultureAsync(deletedCulture: CreateStorageCulture(newCulture: culture));

    }, isValueTask: true);

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

    private IQueryable<Culture> ExecuteGetAllCulture(bool ignoreFilters = false) =>
        (ignoreFilters
            ? cultureBroker.GetAllCulturesIgnoringFilters()
            : cultureBroker.GetAllCultures());

    private Culture ExecuteGetCulture(string cultureId, bool ignoreFilters = false)
    {
        ValidateId(cultureId: cultureId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllCulture(ignoreFilters: true)
                .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);
        }

        Culture culture = ExecuteGetAllCulture()
            .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);

        if ((object)culture != null)
        {
            return culture;
        }

        Culture culture2 = ExecuteGetAllCulture(ignoreFilters: true)
            .FirstOrDefault(predicate: (Culture i) => i.Id == cultureId);

        if ((object)culture2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}