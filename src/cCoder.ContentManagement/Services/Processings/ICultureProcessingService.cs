using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ICultureProcessingService
{
    Culture Get(string id);

    IQueryable<Culture> GetAll(bool ignoreFilters = false);

    ValueTask<Culture> AddAsync(Culture entity);

    ValueTask<Culture> UpdateAsync(Culture entity);

    ValueTask DeleteAsync(string id);

    ValueTask<IEnumerable<Result<Culture>>> AddOrUpdate(IEnumerable<Culture> items);

    ValueTask DeleteAllAsync(IEnumerable<Culture> items);
}
