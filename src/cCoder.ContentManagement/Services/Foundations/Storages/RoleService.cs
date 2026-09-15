// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class RoleService(IRoleBroker roleBroker) : IRoleService
{
    public IQueryable<Role> GetAllRolesIgnoringFilters() =>
        TryCatch(operation: () =>
    {
        return roleBroker.GetAllRolesIgnoringFilters();
    });

    public ValueTask<Role> AddRoleAsync(Role newRole) =>
        TryCatch<Role>(operation: async () =>
    {
        ValidateRoleOnAdd(inputs: [newRole]);
        ValidateRole(role: newRole, parameterName: "newRole");

        Role storedRole = await roleBroker.AddRoleAsync(
            newRole: CreateStorageRole(role: newRole));

        newRole.Id = storedRole.Id;
        return newRole;
    }, isValueTask: true);

    public ValueTask<Role> UpdateRoleAsync(Role updatedRole) =>
        TryCatch<Role>(operation: async () =>
    {
        ValidateRoleOnUpdate(inputs: [updatedRole]);
        ValidateRole(role: updatedRole, parameterName: "updatedRole");

        Role storedRole = await roleBroker.UpdateRoleAsync(
            updatedRole: CreateStorageRole(role: updatedRole));

        updatedRole.Id = storedRole.Id;
        return updatedRole;
    }, isValueTask: true);

    private static Role CreateStorageRole(Role role) =>
        new()
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs
        };
}