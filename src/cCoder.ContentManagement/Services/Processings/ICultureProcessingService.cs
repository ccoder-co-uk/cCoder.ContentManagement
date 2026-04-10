using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Processings;

public interface ICultureProcessingService
{
    Culture Get(string id);

    IQueryable<Culture> GetAll(bool ignoreFilters = false);

    ValueTask<Culture> AddAsync(Culture entity);

    ValueTask<Culture> UpdateAsync(Culture entity);

    ValueTask DeleteAsync(string id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Culture>>> AddOrUpdate(IEnumerable<Culture> items);

    ValueTask DeleteAllAsync(IEnumerable<Culture> items);
}
