// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IUserRoleService
{
    IQueryable<UserRole> GetAllUserRolesIgnoringFilters();

    ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole);

    ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRoles);
}