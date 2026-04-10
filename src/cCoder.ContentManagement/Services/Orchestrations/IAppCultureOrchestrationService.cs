using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IAppCultureOrchestrationService
{
    IQueryable<AppCulture> GetAll(bool ignoreFilters = false);

    ValueTask<AppCulture> AddAsync(AppCulture entity);

    ValueTask DeleteAsync(AppCulture entity);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<AppCulture>>> AddOrUpdate(IEnumerable<AppCulture> items);

    ValueTask DeleteAllAsync(IEnumerable<AppCulture> items);
}
