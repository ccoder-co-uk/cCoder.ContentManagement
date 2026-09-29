// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IRoleBroker
{
    IQueryable<Role> GetAllRoles();

    IQueryable<Role> GetAllRolesIgnoringFilters();

    ValueTask<Role> AddRoleAsync(Role newRole);

    ValueTask<Role> UpdateRoleAsync(Role updatedRole);

    ValueTask<int> DeleteRoleAsync(Role deletedRole);

    ValueTask DeleteAllRolesAsync(IEnumerable<Role> deletedRole);
}