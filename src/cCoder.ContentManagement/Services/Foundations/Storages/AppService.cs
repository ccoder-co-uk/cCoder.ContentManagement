// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.HttpContexts;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppService(
    IAppBroker appBroker,
    IAuthorizationManager authorizationManager,
    ICultureBroker cultureBroker,
    IPrivilegeBroker privilegeBroker,
    IRoleBroker roleBroker,
    IUserRoleBroker userRoleBroker,
    IPageBroker pageBroker,
    IHttpContextBroker httpContextBroker) : IAppService
{
    public AppOperation GetVisibleAppsAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateVisibleAppsAppOperationOnGet(inputs: [appOperation]);
        appOperation.Apps = appBroker.GetAllApps();
        return appOperation;
    });

    public AppOperation GetUnfilteredAppsAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateUnfilteredAppsAppOperationOnGet(inputs: [appOperation]);
        appOperation.Apps = appBroker.GetAllAppsIgnoringFilters();
        return appOperation;
    });

    public AppOperation GetVisibleAppAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateVisibleAppAppOperationOnGet(inputs: [appOperation]);

        appOperation.App = appBroker.GetAllApps()
            .FirstOrDefault(predicate: app => app.Id == appOperation.AppId);

        return appOperation;
    });

    public AppOperation GetUnfilteredAppAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateUnfilteredAppAppOperationOnGet(inputs: [appOperation]);

        appOperation.App = appBroker.GetAllAppsIgnoringFilters()
            .FirstOrDefault(predicate: app => app.Id == appOperation.AppId);

        return appOperation;
    });

    public ValueTask<AppOperation> GetAppForRenderAppOperationAsync(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppForRenderAppOperationOnGet(inputs: [appOperation]);
        appOperation.App = await appBroker.GetAppForRenderAsync(appId: appOperation.AppId);
        return appOperation;
    }, isValueTask: true);

    public AppOperation GetAppForDeleteAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateAppForDeleteAppOperationOnGet(inputs: [appOperation]);
        appOperation.App = appBroker.GetAppForDelete(appId: appOperation.AppId);
        return appOperation;
    });

    public ValueTask<AppOperation> AddAppOperationAsync(AppOperation newAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppOperationOnAdd(inputs: [newAppOperation]);
        App storageApp = CreateStorageApp(newApp: newAppOperation.App);
        App result = await appBroker.AddAppAsync(newApp: storageApp);
        CopyApp(source: result, destination: newAppOperation.App);
        return newAppOperation;
    }, isValueTask: true);

    public ValueTask<AppOperation> UpdateAppOperationAsync(AppOperation updatedAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppOperationOnUpdate(inputs: [updatedAppOperation]);
        App storageApp = CreateStorageApp(newApp: updatedAppOperation.App);
        App result = await appBroker.UpdateAppAsync(updatedApp: storageApp);
        CopyApp(source: result, destination: updatedAppOperation.App);
        return updatedAppOperation;
    }, isValueTask: true);

    public ValueTask<AppOperation> DeleteAppOperationAsync(AppOperation deletedAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppOperationOnDelete(inputs: [deletedAppOperation]);
        await appBroker.DeleteAppAggregateAsync(deletedApp: deletedAppOperation.App);
        return deletedAppOperation;
    }, isValueTask: true);

    public AppOperation GetRequestPathAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateRequestPathAppOperationOnGet(inputs: [appOperation]);
        appOperation.Text = httpContextBroker.GetRequestPath();
        return appOperation;
    });

    public AppOperation GetRequestHostAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateRequestHostAppOperationOnGet(inputs: [appOperation]);
        appOperation.Text = httpContextBroker.GetRequestHost();
        return appOperation;
    });

    public AppOperation GetCulturesAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateCulturesAppOperationOnGet(inputs: [appOperation]);
        appOperation.Cultures = cultureBroker.GetAllCultures();
        return appOperation;
    });

    public AppOperation GetPrivilegesAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidatePrivilegesAppOperationOnGet(inputs: [appOperation]);
        appOperation.Privileges = privilegeBroker.GetAllPrivileges();
        return appOperation;
    });

    public AppOperation GetCurrentUserAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateCurrentUserAppOperationOnGet(inputs: [appOperation]);
        appOperation.Text = authorizationManager.GetCurrentUser()?.Id;
        return appOperation;
    });

    public AppOperation GetCurrentUserIdAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateCurrentUserIdAppOperationOnGet(inputs: [appOperation]);
        appOperation.Text = authorizationManager.GetCurrentUserId();
        return appOperation;
    });

    public AppOperation IsAdminOfAppAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateAdminOfAppAppOperationOnGet(inputs: [appOperation]);
        appOperation.Result = authorizationManager.IsAdminOfApp(appId: appOperation.AppId);
        return appOperation;
    });

    public AppOperation AuthorizeAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateAppOperationOnAuthorize(inputs: [appOperation]);
        authorizationManager.Authorize(appId: appOperation.OptionalAppId, privilege: appOperation.Privilege);
        return appOperation;
    });

    public ValueTask<AppOperation> AddRoleAppOperationAsync(AppOperation newAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateRoleAppOperationOnAdd(inputs: [newAppOperation]);
        Role storageRole = CreateStorageRole(role: newAppOperation.Role);
        newAppOperation.Role = await roleBroker.AddRoleAsync(newRole: storageRole);
        return newAppOperation;
    }, isValueTask: true);

    public ValueTask<AppOperation> UpdateRoleAppOperationAsync(AppOperation updatedAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateRoleAppOperationOnUpdate(inputs: [updatedAppOperation]);
        Role storageRole = CreateStorageRole(role: updatedAppOperation.Role);
        updatedAppOperation.Role = await roleBroker.UpdateRoleAsync(updatedRole: storageRole);
        return updatedAppOperation;
    }, isValueTask: true);

    public AppOperation GetRolesAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateRolesAppOperationOnGet(inputs: [appOperation]);
        appOperation.Roles = roleBroker.GetAllRolesIgnoringFilters();
        return appOperation;
    });

    public ValueTask<AppOperation> AddUserRoleAppOperationAsync(AppOperation newAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateUserRoleAppOperationOnAdd(inputs: [newAppOperation]);
        UserRole storageUserRole = CreateStorageUserRole(userRole: newAppOperation.UserRole);
        newAppOperation.UserRole = await userRoleBroker.AddUserRoleAsync(newUserRole: storageUserRole);
        return newAppOperation;
    }, isValueTask: true);

    public AppOperation GetUserRolesAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateUserRolesAppOperationOnGet(inputs: [appOperation]);
        appOperation.UserRoles = userRoleBroker.GetAllUserRolesIgnoringFilters();
        return appOperation;
    });

    public ValueTask<AppOperation> DeleteUserRolesAppOperationAsync(AppOperation deletedAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateUserRolesAppOperationOnDelete(inputs: [deletedAppOperation]);
        await userRoleBroker.DeleteAllUserRolesAsync(deletedUserRole: deletedAppOperation.DeletedUserRoles);
        return deletedAppOperation;
    }, isValueTask: true);

    public AppOperation GetPagesAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidatePagesAppOperationOnGet(inputs: [appOperation]);
        appOperation.Pages = pageBroker.GetAllPagesIgnoringFilters();
        return appOperation;
    });

    public ValueTask<AppOperation> UpdatePageAppOperationAsync(AppOperation updatedAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidatePageAppOperationOnUpdate(inputs: [updatedAppOperation]);
        updatedAppOperation.Page = await pageBroker.UpdatePageAsync(updatedPage: updatedAppOperation.Page);
        return updatedAppOperation;
    }, isValueTask: true);

    private static App CreateStorageApp(App newApp) =>
        newApp == null ? null : new App
        {
            Id = newApp.Id,
            DefaultCultureId = newApp.DefaultCultureId,
            TenantId = newApp.TenantId,
            Name = newApp.Name,
            Domain = newApp.Domain,
            DefaultTheme = newApp.DefaultTheme,
            ConfigJson = newApp.ConfigJson,
        };

    private static Role CreateStorageRole(Role role) =>
        role == null ? null : new Role
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs
        };

    private static UserRole CreateStorageUserRole(UserRole userRole) =>
        userRole == null ? null : new UserRole
        {
            RoleId = userRole.RoleId,
            UserId = userRole.UserId
        };

    private static void CopyApp(App source, App destination)
    {
        destination.Id = source.Id;
        destination.DefaultCultureId = source.DefaultCultureId;
        destination.TenantId = source.TenantId;
        destination.Name = source.Name;
        destination.Domain = source.Domain;
        destination.DefaultTheme = source.DefaultTheme;
        destination.ConfigJson = source.ConfigJson;
    }
}