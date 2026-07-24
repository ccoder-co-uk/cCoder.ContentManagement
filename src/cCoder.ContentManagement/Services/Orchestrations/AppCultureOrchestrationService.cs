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
    public IQueryable<AppCulture> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<AppCulture> AddAsync(AppCulture entity)
    {
        ValidateAppCulture(appCulture: entity, parameterName: "entity");

        AppCulture result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseAppCultureAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(AppCulture entity)
    {
        ValidateAppCulture(appCulture: entity, parameterName: "entity");

        await eventService.RaiseAppCultureDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(entity: entity);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        AppCulture[] appCulturesToDelete =
            [.. GetAll(ignoreFilters: true)
            .Where(predicate: appCulture => appCulture.AppId == appId)];

        foreach (AppCulture appCulture in appCulturesToDelete)
        {
            await DeleteAsync(entity: appCulture);
        }
    }

    public async ValueTask<IEnumerable<Result<AppCulture>>> AddOrUpdate(IEnumerable<AppCulture> items)
    {
        AppCulture[] appCultures = ValidateAppCultures(appCultures: items, parameterName: "items")
            .ToArray();

        List<Result<AppCulture>> results = new();

        foreach (AppCulture appCulture in appCultures)
        {
            try
            {
                AppCulture existingAppCulture = GetAll(ignoreFilters: true)
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

                AppCulture result = await AddAsync(entity: appCulture);

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

    public async ValueTask DeleteAllAsync(IEnumerable<AppCulture> items)
    {
        AppCulture[] appCultures = ValidateAppCultures(appCultures: items, parameterName: "items")
            .ToArray();

        foreach (AppCulture appCulture in appCultures)
        {
            await DeleteAsync(entity: appCulture);
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