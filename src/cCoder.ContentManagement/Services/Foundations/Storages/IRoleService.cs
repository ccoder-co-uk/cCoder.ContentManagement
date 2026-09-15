// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IRoleService
{
    IQueryable<Role> GetAllRolesIgnoringFilters();

    ValueTask<Role> AddRoleAsync(Role newRole);

    ValueTask<Role> UpdateRoleAsync(Role updatedRole);
}