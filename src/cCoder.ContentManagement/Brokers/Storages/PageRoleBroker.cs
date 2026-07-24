// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Include(navigationPropertyPath: pageRole => pageRole.Role)
            : coreDataContext.PageRoles
                .Include(navigationPropertyPath: pageRole => pageRole.Role);
    }

    public async ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        PageRole result = (await coreDataContext.PageRoles.AddAsync(entity: newPageRole)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePageRoleAsync(PageRole deletedPageRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageRoles.Remove(entity: deletedPageRole);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> deletedPageRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.PageRoles.RemoveRange(entities: deletedPageRole);
        await coreDataContext.SaveChangesAsync();
    }
}