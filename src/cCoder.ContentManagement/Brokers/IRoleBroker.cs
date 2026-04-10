using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IRoleBroker
{
    IQueryable<Role> GetAllRoles(bool ignoreFilters);

    ValueTask<Role> AddRoleAsync(Role entity);

    ValueTask<Role> UpdateRoleAsync(Role entity);

    ValueTask<int> DeleteRoleAsync(Role entity);

    ValueTask DeleteAllRolesAsync(IEnumerable<Role> items);
}
