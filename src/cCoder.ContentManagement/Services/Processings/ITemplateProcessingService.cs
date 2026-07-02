using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ITemplateProcessingService
{
    Template Get(int id);

    IQueryable<Template> GetAll(bool ignoreFilters = false);

    ValueTask<Template> AddAsync(Template entity);

    ValueTask<Template> UpdateAsync(Template entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<Template>>> AddOrUpdate(IEnumerable<Template> items);

    ValueTask DeleteAllAsync(IEnumerable<Template> items);
}
