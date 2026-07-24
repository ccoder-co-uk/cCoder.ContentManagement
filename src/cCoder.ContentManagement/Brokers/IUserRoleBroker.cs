// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IUserRoleBroker
{
    IQueryable<UserRole> GetAllUserRoles(bool ignoreFilters);

    ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole);

    ValueTask<int> DeleteUserRoleAsync(UserRole deletedUserRole);

    ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRole);
}