// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class AppCultureOrchestrationService(
    IAppCultureProcessingService processingService,
    IAppCultureEventProcessingService eventService) : IAppCultureOrchestrationService
{
    public IQueryable<AppCulture> GetAllAppCulture(bool ignoreFilters = false) =>
        processingService.GetAllAppCulture(ignoreFilters: ignoreFilters);

    public async ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture)
    {
        ValidateAppCulture(appCulture: newAppCulture, parameterName: "entity");

        AppCulture result = await processingService.AddAppCultureAsync(newAppCulture: newAppCulture);
        await eventService.RaiseAppCultureAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAppCultureAsync(AppCulture deletedAppCulture)
    {
        ValidateAppCulture(appCulture: deletedAppCulture, parameterName: "entity");

        await eventService.RaiseAppCultureDeleteEventAsync(entity: deletedAppCulture);
        await processingService.DeleteAppCultureAsync(deletedAppCulture: deletedAppCulture);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        AppCulture[] appCulturesToDelete =
            [.. GetAllAppCulture(ignoreFilters: true)
            .Where(predicate: appCulture => appCulture.AppId == appId)];

        foreach (AppCulture appCulture in appCulturesToDelete)
        {
            await DeleteAppCultureAsync(deletedAppCulture: appCulture);
        }
    }

    public async ValueTask<IEnumerable<Result<AppCulture>>> AddOrUpdateAppCultureResult(IEnumerable<AppCulture> newAppCulture)
    {
        AppCulture[] appCultures = ValidateAppCultures(appCultures: newAppCulture, parameterName: "items")
            .ToArray();

        List<Result<AppCulture>> results = new();

        foreach (AppCulture appCulture in appCultures)
        {
            try
            {
                AppCulture existingAppCulture = GetAllAppCulture(ignoreFilters: true)
                    .FirstOrDefault(predicate: existing =>
                        existing.AppId == appCulture.AppId &&
                        existing.CultureId == appCulture.CultureId);

                if (existingAppCulture != null)
                {
                    results.Add(item: new Result<AppCulture>
                    {
                        Id = $"{appCulture.AppId}:{appCulture.CultureId}",
                        Success = true,
                        Item = existingAppCulture,
                        Message = "Already Exists"
                    });

                    continue;
                }

                AppCulture result = await AddAppCultureAsync(newAppCulture: appCulture);

                results.Add(item: new Result<AppCulture>
                {
                    Id = $"{appCulture.AppId}:{appCulture.CultureId}",
                    Success = true,
                    Item = result,
                    Message = "Added Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<AppCulture>
                {
                    Id = $"{appCulture.AppId}:{appCulture.CultureId}",
                    Success = false,
                    Item = appCulture,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAppCultureAsync(IEnumerable<AppCulture> deletedAppCulture)
    {
        AppCulture[] appCultures = ValidateAppCultures(appCultures: deletedAppCulture, parameterName: "items")
            .ToArray();

        foreach (AppCulture appCulture in appCultures)
        {
            await DeleteAppCultureAsync(deletedAppCulture: appCulture);
        }
    }

    private static AppCulture ValidateAppCulture(AppCulture appCulture, string parameterName)
    {
        if (appCulture == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return appCulture;
    }

    private static IEnumerable<AppCulture> ValidateAppCultures(IEnumerable<AppCulture> appCultures, string parameterName)
    {
        if (appCultures == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return appCultures;
    }
}