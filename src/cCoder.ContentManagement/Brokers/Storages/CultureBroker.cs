// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class CultureBroker(ICoreContextFactory coreContextFactory) : ICultureBroker
{
    public IQueryable<Culture> GetAllCultures(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Cultures.IgnoreQueryFilters()
            : coreDataContext.Cultures;
    }

    public async ValueTask<Culture> AddCultureAsync(Culture newCulture)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Culture result = (await coreDataContext.Cultures.AddAsync(entity: newCulture)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Culture result = coreDataContext.Cultures.Update(entity: updatedCulture)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCultureAsync(Culture deletedCulture)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Cultures.Remove(entity: deletedCulture);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCulturesAsync(IEnumerable<Culture> deletedCulture)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Cultures.RemoveRange(entities: deletedCulture);
        await coreDataContext.SaveChangesAsync();
    }
}