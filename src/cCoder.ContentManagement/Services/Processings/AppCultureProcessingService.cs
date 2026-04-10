using cCoder.ContentManagement.Services.Foundations.Storages;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Services.Processings;

internal class AppCultureProcessingService(IAppCultureService service) : IAppCultureProcessingService
{
    public IQueryable<AppCulture> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public async ValueTask<AppCulture> AddAsync(AppCulture entity)
    {
        try
        {
            return await service.AddAsync(entity);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException("The app culture must reference an existing app and culture.", ex);
        }
    }

    public async ValueTask DeleteAsync(AppCulture link)
    {
        AppCulture dbVersion = service.Get(link.AppId, link.CultureId);
        if (dbVersion == null)
        {
            throw new InvalidOperationException("The app culture does not exist.");
        }

        await service.DeleteAsync(dbVersion);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<AppCulture>>> AddOrUpdate(IEnumerable<AppCulture> items)
    {
        List<cCoder.ContentManagement.Models.Result<AppCulture>> results = [];

        foreach (AppCulture item in items)
        {
            try
            {
                AppCulture existing = service.Get(item.AppId, item.CultureId, ignoreFilters: true);

                results.Add(new cCoder.ContentManagement.Models.Result<AppCulture>
                {
                    Id = $"{item.AppId}:{item.CultureId}",
                    Success = true,
                    Item = existing ?? await AddAsync(item),
                    Message = existing == null ? "Added Successfully" : "Already Exists"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<AppCulture>
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
            await DeleteAsync(item);
        }
    }
}
