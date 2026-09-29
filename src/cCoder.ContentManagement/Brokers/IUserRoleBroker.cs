// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Brokers;

public interface IUserRoleBroker
{
    IQueryable<UserRole> GetAllUserRoles();

    IQueryable<UserRole> GetAllUserRolesIgnoringFilters();

    ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole);

    ValueTask<int> DeleteUserRoleAsync(UserRole deletedUserRole);

    ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRole);
}