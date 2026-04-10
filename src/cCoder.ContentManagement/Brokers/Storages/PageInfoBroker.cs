using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class PageInfoBroker(ICoreContextFactory coreContextFactory) : IPageInfoBroker
{
    public IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.PageInfo.IgnoreQueryFilters()
            : coreDataContext.PageInfo;
    }

    public async ValueTask<PageInfo> AddPageInfoAsync(PageInfo entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PageInfo result = (await coreDataContext.PageInfo.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PageInfo result = coreDataContext.PageInfo.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePageInfoAsync(PageInfo entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageInfo.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageInfo.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
