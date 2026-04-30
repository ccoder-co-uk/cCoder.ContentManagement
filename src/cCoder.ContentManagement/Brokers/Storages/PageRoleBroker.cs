using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class PageRoleBroker(ICoreContextFactory coreContextFactory) : IPageRoleBroker
{
    public IQueryable<PageRole> GetAllPageRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.PageRoles
                .IgnoreQueryFilters()
                .Include(pageRole => pageRole.Role)
            : coreDataContext.PageRoles
                .Include(pageRole => pageRole.Role);
    }

    public async ValueTask<PageRole> AddPageRoleAsync(PageRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PageRole result = (await coreDataContext.PageRoles.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePageRoleAsync(PageRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageRoles.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageRoles.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }
}
