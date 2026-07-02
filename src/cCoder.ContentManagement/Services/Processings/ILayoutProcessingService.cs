using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ILayoutProcessingService
{
    Layout Get(int id);

    IQueryable<Layout> GetAll(bool ignoreFilters = false);

    ValueTask<Layout> AddAsync(Layout entity);

    ValueTask<Layout> UpdateAsync(Layout entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items);

    ValueTask DeleteAllAsync(IEnumerable<Layout> items);
}
