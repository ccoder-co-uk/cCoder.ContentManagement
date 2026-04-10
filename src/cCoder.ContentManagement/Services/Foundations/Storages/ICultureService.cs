using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ICultureService
{
    Culture Get(string id, bool ignoreFilters = false);

    IQueryable<Culture> GetAll(bool ignoreFilters = false);

    ValueTask<Culture> AddAsync(Culture culture);

    ValueTask<Culture> UpdateAsync(Culture culture);

    ValueTask DeleteAsync(string id);
}
