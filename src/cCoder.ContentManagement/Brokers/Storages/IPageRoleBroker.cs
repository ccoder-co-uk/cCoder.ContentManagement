using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IPageRoleBroker
{
    IQueryable<PageRole> GetAllPageRoles(bool ignoreFilters);

    ValueTask<PageRole> AddPageRoleAsync(PageRole entity);

    ValueTask<int> DeletePageRoleAsync(PageRole entity);

    ValueTask DeleteAllPageRolesAsync(IEnumerable<PageRole> items);
}
