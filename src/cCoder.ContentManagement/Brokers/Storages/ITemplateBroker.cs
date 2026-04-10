using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ITemplateBroker
{
    IQueryable<Template> GetAllTemplates(bool ignoreFilters);

    ValueTask<Template> AddTemplateAsync(Template entity);

    ValueTask<Template> UpdateTemplateAsync(Template entity);

    ValueTask<int> DeleteTemplateAsync(Template entity);

    ValueTask DeleteAllTemplatesAsync(IEnumerable<Template> items);
}
