// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class TemplateBroker(ICoreContextFactory coreContextFactory) : ITemplateBroker
{
    public IQueryable<Template> GetAllTemplates(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Dependencies.QueryFilterDependency.Apply(
            query: coreDataContext.Templates,
            ignoreFilters: ignoreFilters);
    }

    public async ValueTask<Template> AddTemplateAsync(Template newTemplate)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Template result = (await coreDataContext.Templates.AddAsync(entity: newTemplate)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Template> UpdateTemplateAsync(Template updatedTemplate)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Template result = coreDataContext.Templates.Update(entity: updatedTemplate)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteTemplateAsync(Template deletedTemplate)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Templates.Remove(entity: deletedTemplate);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllTemplatesAsync(IEnumerable<Template> deletedTemplate)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Templates.RemoveRange(entities: deletedTemplate);
        await coreDataContext.SaveChangesAsync();
    }

}