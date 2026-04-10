using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IAppCultureService
{
    IQueryable<AppCulture> GetAll(bool ignoreFilters = false);

    AppCulture Get(int appId, string cultureId, bool ignoreFilters = false);

    ValueTask<AppCulture> AddAsync(AppCulture appCulture);

    ValueTask DeleteAsync(AppCulture appCulture);
}
