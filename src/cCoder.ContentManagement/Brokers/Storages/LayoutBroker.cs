// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class LayoutBroker(ICoreContextFactory coreContextFactory) : ILayoutBroker
{
    public IQueryable<Layout> GetAllLayouts(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Layouts.IgnoreQueryFilters()
            : coreDataContext.Layouts;
    }

    public async ValueTask<Layout> AddLayoutAsync(Layout newLayout)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Layout result = (await coreDataContext.Layouts.AddAsync(entity: newLayout)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Layout result = coreDataContext.Layouts.Update(entity: updatedLayout)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteLayoutAsync(Layout deletedLayout)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Layouts.Remove(entity: deletedLayout);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllLayoutsAsync(IEnumerable<Layout> deletedLayout)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Layouts.RemoveRange(entities: deletedLayout);
        await coreDataContext.SaveChangesAsync();
    }

}