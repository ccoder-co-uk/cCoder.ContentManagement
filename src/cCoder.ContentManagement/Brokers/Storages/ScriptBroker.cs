using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class ScriptBroker(ICoreContextFactory coreContextFactory) : IScriptBroker
{
    public IQueryable<Script> GetAllScripts(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Scripts.IgnoreQueryFilters()
            : coreDataContext.Scripts;
    }

    public async ValueTask<Script> AddScriptAsync(Script entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Script result = (await coreDataContext.Scripts.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Script> UpdateScriptAsync(Script entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Script result = coreDataContext.Scripts.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteScriptAsync(Script entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Scripts.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllScriptsAsync(IEnumerable<Script> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Scripts.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }

}
