// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using Microsoft.EntityFrameworkCore;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class AppCultureProcessingService(IAppCultureService service) : IAppCultureProcessingService
{
    public IQueryable<AppCulture> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<AppCulture> AddAsync(AppCulture entity)
    {
        try
        {
            return await service.AddAsync(appCulture: entity);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException?.Message.Contains(value: "FOREIGN KEY", comparisonType: StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException(message: "The app culture must reference an existing app and culture.", innerException: ex);
        }
    }

    public async ValueTask DeleteAsync(AppCulture link)
    {
        AppCulture dbVersion = service.Get(appId: link.AppId, cultureId: link.CultureId);

        if (dbVersion == null)
        {
            throw new InvalidOperationException(message: "The app culture does not exist.");
        }

        await service.DeleteAsync(appCulture: dbVersion);
    }

    public async ValueTask<IEnumerable<Result<AppCulture>>> AddOrUpdate(IEnumerable<AppCulture> items)
    {
        List<Result<AppCulture>> results = [];

        foreach (AppCulture item in items)
        {
            try
            {
                AppCulture existing = service.Get(appId: item.AppId, cultureId: item.CultureId, ignoreFilters: true);

                results.Add(item: new Result<AppCulture>
                {
                    Id = $"{item.AppId}:{item.CultureId}",
                    Success = true,
                    Item = existing ?? await AddAsync(entity: item),
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
    }

    public async ValueTask DeleteAllAsync(IEnumerable<AppCulture> items)
    {
        foreach (AppCulture item in items)
        {
            await DeleteAsync(link: item);
        }
    }
}