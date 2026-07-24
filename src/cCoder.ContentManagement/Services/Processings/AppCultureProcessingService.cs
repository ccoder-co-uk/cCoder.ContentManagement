// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using Microsoft.EntityFrameworkCore;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppCultureProcessingService(IAppCultureService service) : IAppCultureProcessingService
{
    public IQueryable<AppCulture> GetAllAppCulture(bool ignoreFilters = false) =>
        TryCatch<IQueryable<AppCulture>>(operation: () =>
    {
        ValidateAllAppCultureOnGet(inputs: [ignoreFilters]);
        return service.GetAllAppCulture(ignoreFilters: ignoreFilters);
    });

    public ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture) =>
        TryCatch<AppCulture>(operation: async () =>
    {
        ValidateAppCultureOnAdd(inputs: [newAppCulture]);

        try
        {
            return await service.AddAppCultureAsync(newAppCulture: newAppCulture);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException?.Message.Contains(value: "FOREIGN KEY", comparisonType: StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException(message: "The app culture must reference an existing app and culture.", innerException: ex);
        }

    }, isValueTask: true);

    public ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture) =>
        TryCatch(operation: async () =>
    {
        ValidateAppCultureOnDelete(inputs: [deletedAppCulture]);
        AppCulture dbVersion = service.GetAppCulture(appId: deletedAppCulture.AppId, cultureId: deletedAppCulture.CultureId);

        if (dbVersion == null)
        {
            throw new InvalidOperationException(message: "The app culture does not exist.");
        }

        await service.DeleteAppCultureAsync(deletedAppCulture: dbVersion);

    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<AppCulture>>> AddOrUpdateAppCultureResult(IEnumerable<AppCulture> newAppCulture) =>
        TryCatch<IEnumerable<Result<AppCulture>>>(operation: async () =>
    {
        ValidateOrUpdateAppCultureResultOnAdd(inputs: [newAppCulture]);
        List<Result<AppCulture>> results = [];

        foreach (AppCulture item in newAppCulture)
        {
            try
            {
                AppCulture existing = service.GetAppCulture(appId: item.AppId, cultureId: item.CultureId, ignoreFilters: true);

                results.Add(item: new Result<AppCulture>
                {
                    Id = $"{item.AppId}:{item.CultureId}",
                    Success = true,
                    Item = existing ?? await ExecuteAddAppCultureAsync(newAppCulture: item),
                    Message = existing == null ? "Added Successfully" : "Already Exists"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<AppCulture>
                {
                    Id = $"{item.AppId}:{item.CultureId}",
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllAppCultureAsync(IEnumerable<AppCulture> deletedAppCulture) =>
        TryCatch(operation: async () =>
    {
        ValidateAllAppCultureOnDelete(inputs: [deletedAppCulture]);

        foreach (AppCulture item in deletedAppCulture)
        {
            await ExecuteDeleteAppCultureAsync(deletedAppCulture: item);
        }

    }, isValueTask: true);

    private async ValueTask<AppCulture> ExecuteAddAppCultureAsync(AppCulture newAppCulture)
    {
        try
        {
            return await service.AddAppCultureAsync(newAppCulture: newAppCulture);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException?.Message.Contains(value: "FOREIGN KEY", comparisonType: StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException(message: "The app culture must reference an existing app and culture.", innerException: ex);
        }
    }

    private async ValueTask ExecuteDeleteAppCultureAsync(AppCulture deletedAppCulture)
    {
        AppCulture dbVersion = service.GetAppCulture(appId: deletedAppCulture.AppId, cultureId: deletedAppCulture.CultureId);

        if (dbVersion == null)
        {
            throw new InvalidOperationException(message: "The app culture does not exist.");
        }

        await service.DeleteAppCultureAsync(deletedAppCulture: dbVersion);
    }
}