// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PageBroker(ICoreContextFactory coreContextFactory) : IPageBroker
{
    public IQueryable<Page> GetAllPages(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Pages.IgnoreQueryFilters()
            : coreDataContext.Pages;
    }

    public async ValueTask<Page> AddPageAsync(Page newPage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Page result = (await coreDataContext.Pages.AddAsync(entity: newPage)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Page> UpdatePageAsync(Page updatedPage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Page result = coreDataContext.Pages.Update(entity: updatedPage)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePageAsync(Page deletedPage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Pages.Remove(entity: deletedPage);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPagesAsync(IEnumerable<Page> deletedPage)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Pages.RemoveRange(entities: deletedPage);
        await coreDataContext.SaveChangesAsync();
    }
}