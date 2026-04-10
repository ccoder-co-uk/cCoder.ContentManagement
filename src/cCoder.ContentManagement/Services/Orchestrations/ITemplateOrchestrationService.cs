using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ITemplateOrchestrationService
{
    Template Get(int id);

    IQueryable<Template> GetAll(bool ignoreFilters = false);

    ValueTask<Template> AddAsync(Template entity);

    ValueTask<Template> UpdateAsync(Template entity);

    ValueTask DeleteAsync(int id);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Template>>> AddOrUpdate(IEnumerable<Template> items);

    ValueTask ImportTemplatesAsync(int appId, Template[] items);

    ValueTask DeleteAllAsync(IEnumerable<Template> items);
}
