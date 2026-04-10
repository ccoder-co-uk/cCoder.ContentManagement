using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ITemplateService
{
    Template Get(int id, bool ignoreFilters = false);

    IQueryable<Template> GetAll(bool ignoreFilters = false);

    ValueTask<Template> AddAsync(Template template);

    ValueTask<Template> UpdateAsync(Template template);

    ValueTask DeleteAsync(int id);
}
