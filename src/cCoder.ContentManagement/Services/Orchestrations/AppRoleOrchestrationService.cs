// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class AppRoleOrchestrationService(
    IRoleService roleService,
    IUserRoleService userRoleService) : IAppRoleOrchestrationService
{
    public ValueTask PersistNewAppRolesAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateNewAppRolesOnPersist(inputs: [app]);
        ArgumentNullException.ThrowIfNull(argument: app);

        foreach (Role role in app.Roles ?? [])
        {
            role.AppId = app.Id;
            role.App = null;

            await roleService.AddRoleAsync(newRole: new Role
            {
                Id = role.Id,
                AppId = role.AppId,
                Name = role.Name,
                Description = role.Description,
                Privs = role.Privs
            });

            foreach (UserRole userRole in role.Users ?? [])
            {
                userRole.RoleId = role.Id;
                userRole.Role = null;

                await userRoleService.AddUserRoleAsync(
                    newUserRole: new UserRole
                    {
                        RoleId = userRole.RoleId,
                        UserId = userRole.UserId
                    });
            }
        }
    }, isValueTask: true);
}