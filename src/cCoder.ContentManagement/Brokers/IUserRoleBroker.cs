using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IUserRoleBroker
{
    IQueryable<UserRole> GetAllUserRoles(bool ignoreFilters);

    ValueTask<UserRole> AddUserRoleAsync(UserRole entity);

    ValueTask<int> DeleteUserRoleAsync(UserRole entity);

    ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> items);
}
