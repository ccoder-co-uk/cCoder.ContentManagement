// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class UserRoleService(
    IUserRoleBroker userRoleBroker) : IUserRoleService
{
    public IQueryable<UserRole> GetAllUserRolesIgnoringFilters() =>
        TryCatch(operation: () =>
    {
        return userRoleBroker.GetAllUserRolesIgnoringFilters();
    });

    public ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole) =>
        TryCatch<UserRole>(operation: () =>
    {
        ValidateUserRoleOnAdd(inputs: [newUserRole]);
        ValidateUserRole(userRole: newUserRole, parameterName: "newUserRole");
        return userRoleBroker.AddUserRoleAsync(newUserRole: newUserRole);
    }, isValueTask: true);

    public ValueTask DeleteAllUserRolesAsync(
        IEnumerable<UserRole> deletedUserRoles) =>
        TryCatch(operation: () =>
    {
        ValidateAllUserRolesOnDelete(inputs: [deletedUserRoles]);
        ArgumentNullException.ThrowIfNull(argument: deletedUserRoles);

        return userRoleBroker.DeleteAllUserRolesAsync(
            deletedUserRole: deletedUserRoles);
    }, isValueTask: true);
}