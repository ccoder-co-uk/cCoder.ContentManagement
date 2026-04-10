using App = cCoder.Data.Models.CMS.App;

namespace cCoder.ContentManagement.Exposures;

public interface IContentManagementAppExposure
{
    App Get(int id, bool ignoreFilters = false);

    App GetByDomain(string domain, bool ignoreFilters = false);

    ValueTask<App> AddAsync(App app);

    ValueTask<App> UpdateAsync(App app);

    ValueTask DeleteAsync(int appId);
}
