// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IRoleBroker
{
    IQueryable<Role> GetAllRoles(bool ignoreFilters);

    ValueTask<Role> AddRoleAsync(Role newRole);

    ValueTask<Role> UpdateRoleAsync(Role updatedRole);

    ValueTask<int> DeleteRoleAsync(Role deletedRole);

    ValueTask DeleteAllRolesAsync(IEnumerable<Role> deletedRole);
}