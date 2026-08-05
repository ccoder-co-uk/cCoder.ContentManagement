// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PageBroker(ICoreContextFactory coreContextFactory) : IPageBroker
{
    public IQueryable<Page> GetAllPages()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Pages;
    }

    public IQueryable<Page> GetAllPagesIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Pages.IgnoreQueryFilters();
    }

    public async ValueTask<Page> GetPageByIdForRenderAsync(int pageId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Pages
            .IgnoreQueryFilters()
            .AsSplitQuery()
            .Include(navigationPropertyPath: page => page.PageInfo)
            .Include(navigationPropertyPath: page => page.Contents)
            .Include(navigationPropertyPath: page => page.Roles)
            .Include(navigationPropertyPath: page => page.App)
                .ThenInclude(navigationPropertyPath: app => app.Layouts)
            .Include(navigationPropertyPath: page => page.App)
                .ThenInclude(navigationPropertyPath: app => app.Templates)
            .Include(navigationPropertyPath: page => page.App)
                .ThenInclude(navigationPropertyPath: app => app.Resources)
            .Include(navigationPropertyPath: page => page.App)
                .ThenInclude(navigationPropertyPath: app => app.Components)
            .Include(navigationPropertyPath: page => page.App)
                .ThenInclude(navigationPropertyPath: app => app.Scripts)
            .Include(navigationPropertyPath: page => page.App)
                .ThenInclude(navigationPropertyPath: app => app.Pages)
                    .ThenInclude(navigationPropertyPath: appPage => appPage.PageInfo)
            .SingleOrDefaultAsync(
                predicate: page => page.Id == pageId);
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