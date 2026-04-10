using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.AppCulture>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class AppCultureOrchestrationService(
    IAppCultureProcessingService processingService,
    IAppCultureEventProcessingService eventService) : IAppCultureOrchestrationService
{
    public IQueryable<AppCulture> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<AppCulture> AddAsync(AppCulture entity)
    {
        ValidateAppCulture(entity, "entity");

        AppCulture result = await processingService.AddAsync(entity);
        await eventService.RaiseAppCultureAddEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(AppCulture entity)
    {
        ValidateAppCulture(entity, "entity");

        await eventService.RaiseAppCultureDeleteEventAsync(entity);
        await processingService.DeleteAsync(entity);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        AppCulture[] appCulturesToDelete =
            [.. GetAll(ignoreFilters: true).Where(appCulture => appCulture.AppId == appId)];

        foreach (AppCulture appCulture in appCulturesToDelete)
        {
            await DeleteAsync(appCulture);
        }
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<AppCulture> items)
    {
        AppCulture[] appCultures = ValidateAppCultures(items, "items").ToArray();
        List<Result> results = new();

        foreach (AppCulture appCulture in appCultures)
        {
            try
            {
                AppCulture existingAppCulture = GetAll(ignoreFilters: true)
                    .FirstOrDefault(existing =>
                        existing.AppId == appCulture.AppId &&
                        existing.CultureId == appCulture.CultureId);

                if (existingAppCulture != null)
                {
                    results.Add(new Result
                    {
                        Id = $"{appCulture.AppId}:{appCulture.CultureId}",
                        Success = true,
                        Item = existingAppCulture,
                        Message = "Already Exists"
                    });

                    continue;
                }

                AppCulture result = await AddAsync(appCulture);
                results.Add(new Result
                {
                    Id = $"{appCulture.AppId}:{appCulture.CultureId}",
                    Success = true,
                    Item = result,
                    Message = "Added Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
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
        AppCulture[] appCultures = ValidateAppCultures(items, "items").ToArray();

        foreach (AppCulture appCulture in appCultures)
        {
            await DeleteAsync(appCulture);
        }
    }

    private static AppCulture ValidateAppCulture(AppCulture appCulture, string parameterName)
    {
        if (appCulture == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return appCulture;
    }

    private static IEnumerable<AppCulture> ValidateAppCultures(IEnumerable<AppCulture> appCultures, string parameterName)
    {
        if (appCultures == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return appCultures;
    }
}
