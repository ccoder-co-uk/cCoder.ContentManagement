// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PageInfoBroker(ICoreContextFactory coreContextFactory) : IPageInfoBroker
{
    public IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Extensions.Data.QueryFilterExtensions.Apply(
            query: coreDataContext.PageInfo,
            ignoreFilters: ignoreFilters);
    }

    public async ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PageInfo result = (await coreDataContext.PageInfo.AddAsync(entity: newPageInfo)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        PageInfo result = coreDataContext.PageInfo.Update(entity: updatedPageInfo)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePageInfoAsync(PageInfo deletedPageInfo)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageInfo.Remove(entity: deletedPageInfo);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageInfo.RemoveRange(entities: deletedPageInfo);
        await coreDataContext.SaveChangesAsync();
    }
}