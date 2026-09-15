// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PageRoleBroker(ICoreContextFactory coreContextFactory) : IPageRoleBroker
{
    public IQueryable<PageRole> GetAllPageRoles()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.PageRoles
            .Include(navigationPropertyPath: pageRole => pageRole.Role);
    }

    public IQueryable<PageRole> GetAllPageRolesIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.PageRoles
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: pageRole => pageRole.Role);
    }

    public PageRole ResolvePageRoleByIds(PageRole pageRole)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        pageRole.Page = coreDataContext.Pages
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: page => page.Id == pageRole.PageId);

        pageRole.Role = coreDataContext.Roles
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: role => role.Id == pageRole.RoleId);

        return pageRole;
    }

    public PageRole ResolvePageRoleByNames(PageRole pageRole)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Page page = coreDataContext.Pages
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: page =>
                page.AppId == pageRole.Page.AppId
                && page.Path == pageRole.Page.Path);

        Role role = coreDataContext.Roles
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: role =>
                role.AppId == pageRole.Role.AppId
                && role.Name == pageRole.Role.Name);

        return new PageRole
        {
            PageId = page?.Id ?? 0,
            RoleId = role?.Id ?? Guid.Empty
        };
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