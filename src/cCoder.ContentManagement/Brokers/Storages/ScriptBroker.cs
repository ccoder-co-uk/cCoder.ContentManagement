// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class ScriptBroker(ICoreContextFactory coreContextFactory) : IScriptBroker
{
    public IQueryable<Script> GetAllScripts(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Dependencies.QueryFilterDependency.Apply(
            query: coreDataContext.Scripts,
            ignoreFilters: ignoreFilters);
    }

    public async ValueTask<Script> AddScriptAsync(Script newScript)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Script result = (await coreDataContext.Scripts.AddAsync(entity: newScript)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Script> UpdateScriptAsync(Script updatedScript)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Script result = coreDataContext.Scripts.Update(entity: updatedScript)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteScriptAsync(Script deletedScript)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Scripts.Remove(entity: deletedScript);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllScriptsAsync(IEnumerable<Script> deletedScript)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Scripts.RemoveRange(entities: deletedScript);
        await coreDataContext.SaveChangesAsync();
    }

}