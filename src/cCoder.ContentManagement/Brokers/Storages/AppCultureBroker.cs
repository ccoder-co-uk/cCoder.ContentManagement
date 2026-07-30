// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class AppCultureBroker(ICoreContextFactory coreContextFactory) : IAppCultureBroker
{
    public IQueryable<AppCulture> GetAllAppCultures()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.AppCultures;
    }

    public IQueryable<AppCulture> GetAllAppCulturesIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.AppCultures.IgnoreQueryFilters();
    }

    public async ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        AppCulture result = (await coreDataContext.AppCultures.AddAsync(entity: newAppCulture)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteAppCultureAsync(AppCulture deletedAppCulture)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.AppCultures.Remove(entity: deletedAppCulture);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllAppCulturesAsync(IEnumerable<AppCulture> deletedAppCulture)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.AppCultures.RemoveRange(entities: deletedAppCulture);
        await coreDataContext.SaveChangesAsync();
    }
}