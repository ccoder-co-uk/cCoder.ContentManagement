using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class TemplateBroker(ICoreContextFactory coreContextFactory) : ITemplateBroker
{
    public IQueryable<Template> GetAllTemplates(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Templates.IgnoreQueryFilters()
            : coreDataContext.Templates;
    }

    public async ValueTask<Template> AddTemplateAsync(Template entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Template result = (await coreDataContext.Templates.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Template> UpdateTemplateAsync(Template entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Template result = coreDataContext.Templates.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteTemplateAsync(Template entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Templates.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllTemplatesAsync(IEnumerable<Template> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Templates.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }

}
