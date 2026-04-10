using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Processings;

public interface ILayoutProcessingService
{
    Layout Get(int id);

    IQueryable<Layout> GetAll(bool ignoreFilters = false);

    ValueTask<Layout> AddAsync(Layout entity);

    ValueTask<Layout> UpdateAsync(Layout entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items);

    ValueTask DeleteAllAsync(IEnumerable<Layout> items);
}
