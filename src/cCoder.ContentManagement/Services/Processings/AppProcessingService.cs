// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppProcessingService(
    IAppService service) : IAppProcessingService
{
    public ValueTask<App> GetAppForRenderAsync(int appId) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppForRenderOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");

        return await GetAppForRenderFromStorageAsync(appId: appId);
    }, isValueTask: true);

    public App GetAppForDelete(int appId) =>
        TryCatch<App>(operation: () =>
    {
        ValidateAppForDeleteOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");
        return GetAppForDeleteFromStorage(appId: appId);
    });

    public App GetApp(int appId) =>
        TryCatch<App>(operation: () =>
    {
        ValidateAppOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");
        return GetAppFromStorage(appId: appId);

    });

    public string GetDomain(int appId, bool ignoreFilters = false) =>
        TryCatch<string>(operation: () =>
    {
        ValidateDomainOnGet(inputs: [appId, ignoreFilters]);
        ValidateId(appId: appId, parameterName: "id");

        return GetAllAppsFromStorage(ignoreFilters: ignoreFilters)
            .Where(predicate: app => app.Id == appId)
            .Select(selector: app => app.Domain)
            .FirstOrDefault();

    });

    public App GetByDomainApp(string domain, bool ignoreFilters = false) =>
        TryCatch<App>(operation: () =>
    {
        ValidateByDomainAppOnGet(inputs: [domain, ignoreFilters]);
        ValidateDomain(domain: domain, parameterName: "domain");

        return GetAllAppsFromStorage(ignoreFilters: ignoreFilters)
            .Where(predicate: app => app.Domain == domain)
            .FirstOrDefault();

    });

    public IQueryable<App> GetAllApp(bool ignoreFilters = false) =>
        TryCatch<IQueryable<App>>(operation: () =>
    {
        ValidateAllAppOnGet(inputs: [ignoreFilters]);
        return GetAllAppsFromStorage(ignoreFilters: ignoreFilters);
    });

    public ValueTask<App> AddAppAsync(App newApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnAdd(inputs: [newApp]);
        ValidateApp(app: newApp, parameterName: "inputApp");

        if (string.IsNullOrEmpty(value: newApp.DefaultTheme))
        {
            newApp.DefaultTheme = "Default";
        }

        newApp.Cultures = BuildCulturesForApp(newApp: newApp);
        newApp.Roles = BuildRolesForApp(app: newApp, isFirstApp: out bool isFirstApp);
        App storedApp = await AddAppToStorageAsync(newApp: newApp, isFirstApp: isFirstApp);

        if (storedApp.Roles != null)
        {
            foreach (Role role in storedApp.Roles)
            {
                role.AppId = storedApp.Id;
                role.App = null;

                await AddRoleAsync(newRole: new Role
                {
                    Id = role.Id,
                    AppId = role.AppId,
                    Name = role.Name,
                    Description = role.Description,
                    Privs = role.Privs,
                });

                if (role.Users == null)
                {
                    continue;
                }

                foreach (UserRole user in role.Users)
                {
                    user.RoleId = role.Id;
                    user.Role = null;

                    await AddUserRoleAsync(newUserRole: new UserRole
                    {
                        RoleId = user.RoleId,
                        UserId = user.UserId,
                    });
                }
            }
        }

        StampAppChildren(app: storedApp);
        return storedApp;

    }, isValueTask: true);

    public ValueTask<App> UpdateAppAsync(App updatedApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnUpdate(inputs: [updatedApp]);
        ValidateApp(app: updatedApp, parameterName: "app");
        App existingApp = GetAppFromStorage(appId: updatedApp.Id, ignoreFilters: true);

        if (existingApp == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        existingApp.DefaultCultureId = updatedApp.DefaultCultureId;
        existingApp.TenantId = updatedApp.TenantId;
        existingApp.Name = updatedApp.Name;
        existingApp.Domain = updatedApp.Domain;
        existingApp.DefaultTheme = updatedApp.DefaultTheme;
        existingApp.ConfigJson = updatedApp.ConfigJson;
        return await UpdateAppInStorageAsync(updatedApp: existingApp);

    }, isValueTask: true);

    public ValueTask DeleteAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");
        await DeleteAppFromStorageAsync(appId: appId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<App>>> AddOrUpdateAppResult(IEnumerable<App> newApp) =>
        TryCatch<IEnumerable<OperationResult<App>>>(operation: async () =>
    {
        ValidateOrUpdateAppResultOnAdd(inputs: [newApp]);
        ValidateApps(apps: newApp, parameterName: "items");
        List<OperationResult<App>> results = [];

        foreach (App item in newApp)
        {
            try
            {
                App app = item.Id < 1
                    ? await ExecuteAddAppAsync(newApp: item)
                    : await ExecuteUpdateAppAsync(app: item);

                results.Add(item: new OperationResult<App>
                {
                    Success = true,
                    Item = app,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<App>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp) =>
        TryCatch(operation: async () =>
    {
        ValidateAllAppOnDelete(inputs: [deletedApp]);
        ValidateApps(apps: deletedApp, parameterName: "items");

        foreach (App item in deletedApp)
        {
            await ExecuteDeleteAsync(appId: item.Id);
        }

    }, isValueTask: true);

    public App ResolveCurrentApp() =>
        TryCatch<App>(operation: () =>
    {
        string text = GetRequestPath();

        if (text.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase) && text.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            int num = text.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
            int num2 = text.IndexOf(value: ')', startIndex: num);

            if (num2 > num)
            {
                int num3 = num;

                if (int.TryParse(s: text.Substring(startIndex: num3, length: num2 - num3), result: out var result))
                {
                    return GetAppFromStorage(appId: result);
                }
            }
        }

        string domain = GetRequestHost();
        return ExecuteGetByDomainApp(domain: domain);

    });

    public ValueTask UpdatePageOrderAppAsync(int key, App updatedApp) =>
        TryCatch(operation: async () =>
    {
        ValidatePageOrderAppOnUpdate(inputs: [key, updatedApp]);
        ValidateId(appId: key, parameterName: "key");
        ValidateApp(app: updatedApp, parameterName: "app");
        Authorize(appId: key, privilege: "App_update");

        Dictionary<int, Page> incomingPagesById =
            (updatedApp.Pages ?? [])
            .ToDictionary(keySelector: page => page.Id);

        Page[] existingPages = GetAllPagesIgnoringFilters()
            .Where(predicate: page => page.AppId == key)
            .ToArray();

        foreach (Page existingPage in existingPages)
        {
            if (incomingPagesById.TryGetValue(
                key: existingPage.Id,
                value: out Page incomingPage))
            {
                existingPage.Order = incomingPage.Order;
                existingPage.ParentId = incomingPage.ParentId;
                await UpdatePageAsync(updatedPage: existingPage);
            }
        }

    }, isValueTask: true);

    private ICollection<AppCulture> BuildCulturesForApp(App newApp)
    {
        IEnumerable<string> enumerable = newApp.Cultures?
            .Select(selector: (AppCulture culture) => culture.CultureId ?? string.Empty)
            ?? Array.Empty<string>();

        string[] requestedCultureIds = enumerable.Distinct()
            .ToArray();

        AppCulture[] culturesForApp = GetAllCultures()
            .Where(predicate: culture => culture.Id == string.Empty || requestedCultureIds.Contains(value: culture.Id))
            .Select(selector: culture => new AppCulture
            {
                CultureId = culture.Id
            })
            .ToArray();

        if (string.IsNullOrEmpty(value: newApp.DefaultCultureId))
        {
            newApp.DefaultCultureId = enumerable.FirstOrDefault() ?? string.Empty;
        }

        return culturesForApp;
    }

    private ICollection<Role> BuildRolesForApp(App app, out bool isFirstApp)
    {
        List<Role> list = (app.Roles ?? new List<Role>()).ToList();

        string currentUserId = GetCurrentUser()?.Id
            ?? GetCurrentUserId();

        bool firstApp = !GetAllAppsFromStorage(ignoreFilters: true)
            .Any();

        isFirstApp = firstApp;

        string defaultUserId = string.IsNullOrWhiteSpace(value: currentUserId) ? "Guest" : currentUserId;

        string bootstrapUserId = firstApp
            ? NormalizeBootstrapUserId(userId: currentUserId)
            : defaultUserId;

        string[] administratorPrivilegeIds = GetAllPrivileges()
            .ToArray()
            .Where(predicate: privilege => firstApp || privilege.Id != "app_create")
            .Select(selector: privilege => privilege.Id)
            .ToArray();

        string[] userPrivilegeIds = GetAllPrivileges()
            .ToArray()
            .Where(predicate: privilege =>
                string.Equals(a: privilege.Operation, b: "Read", comparisonType: StringComparison.OrdinalIgnoreCase) &&
                !privilege.Type.StartsWith(value: "Flow", comparisonType: StringComparison.OrdinalIgnoreCase) &&
                !privilege.Type.StartsWith(value: "Workflow", comparisonType: StringComparison.OrdinalIgnoreCase))
            .Select(selector: privilege => privilege.Id)
            .ToArray();

        EnsureRole(roles: list, roleName: "Administrators", requiredPrivileges: administratorPrivilegeIds, userId: bootstrapUserId);
        EnsureRole(roles: list, roleName: "Users", requiredPrivileges: userPrivilegeIds, userId: bootstrapUserId);
        EnsureRole(roles: list, roleName: "Guests", requiredPrivileges: userPrivilegeIds, userId: "Guest");

        if (firstApp)
        {
            EnsureRole(
                roles: list,
                roleName: "System Admins",
                requiredPrivileges: ["app_create"],
                userId: bootstrapUserId);
        }

        foreach (Role item in list)
        {
            item.App = null;
            item.AppId = app.Id;
            Role role = item;

            if (role.Users == null)
            {
                ICollection<UserRole> collection = (role.Users = new List<UserRole>());
            }

            foreach (UserRole user in item.Users)
            {
                user.RoleId = item.Id;
                user.Role = null;
            }
        }

        return list;
    }

    private static void EnsureRole(ICollection<Role> roles, string roleName, IEnumerable<string> requiredPrivileges, string userId)
    {
        Role role = roles.FirstOrDefault(predicate: (Role foundRole) => string.Equals(a: foundRole.Name, b: roleName, comparisonType: StringComparison.OrdinalIgnoreCase));

        if (role == null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Users = new List<UserRole>(),
                Pages = new List<PageRole>(),
                Privileges = new List<string>()
            };

            roles.Add(item: role);
        }

        Role role2 = role;

        if (role2.Users == null)
        {
            ICollection<UserRole> collection = (role2.Users = new List<UserRole>());
        }

        role2 = role;

        if (role2.Pages == null)
        {
            ICollection<PageRole> collection3 = (role2.Pages = new List<PageRole>());
        }

        Role role3 = role;
        List<string> list = new List<string>();
        list.AddRange(collection: role.Privileges.Union<string>(second: requiredPrivileges, comparer: StringComparer.OrdinalIgnoreCase));
        role3.Privileges = list;
        role.Privs = string.Join(separator: ',', values: role.Privileges);

        if (!string.IsNullOrWhiteSpace(value: userId) && !role.Users.Any(predicate: (UserRole existingUserRole) => existingUserRole.UserId == userId))
        {
            role.Users.Add(item: new UserRole
            {
                RoleId = role.Id,
                UserId = userId
            });
        }
    }

    private static string NormalizeBootstrapUserId(string userId) =>
        string.IsNullOrWhiteSpace(value: userId) || string.Equals(a: userId, b: "Guest", comparisonType: StringComparison.OrdinalIgnoreCase)
            ? null
            : userId;

    private static void StampAppChildren(App app)
    {
        if (app.Cultures != null)
        {
            foreach (AppCulture culture in app.Cultures)
            {
                culture.AppId = app.Id;
            }
        }

        if (app.Pages != null)
        {
            foreach (Page page in app.Pages)
            {
                page.AppId = app.Id;
            }
        }

        if (app.Components != null)
        {
            foreach (Component component in app.Components)
            {
                component.AppId = app.Id;
            }
        }

        if (app.Scripts != null)
        {
            foreach (Script script in app.Scripts)
            {
                script.AppId = app.Id;
            }
        }

        if (app.Roles != null)
        {
            foreach (Role role in app.Roles)
            {
                role.AppId = app.Id;
                role.App = null;

                if (role.Users == null)
                {
                    continue;
                }

                foreach (UserRole user in role.Users)
                {
                    user.RoleId = role.Id;
                    user.Role = null;
                }
            }
        }

        if (app.Templates != null)
        {
            foreach (Template template in app.Templates)
            {
                template.AppId = app.Id;
            }
        }

        if (app.Resources != null)
        {
            foreach (Resource resource in app.Resources)
            {
                resource.AppId = app.Id;
            }
        }

        if (app.Layouts != null)
        {
            foreach (Layout layout in app.Layouts)
            {
                layout.AppId = app.Id;
            }
        }
    }

    private static void ValidateId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (string.IsNullOrWhiteSpace(value: app.Name))
        {
            throw new ValidationException(message: parameterName + ".Name is required.");
        }

        if (string.IsNullOrWhiteSpace(value: app.Domain))
        {
            throw new ValidationException(message: parameterName + ".Domain is required.");
        }
    }

    private static void ValidateApps(IEnumerable<App> apps, string parameterName) =>
        ThrowIf(condition: apps == null, message: parameterName + " is required.");

    private static void ValidateDomain(string domain, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: domain), message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private async ValueTask<App> ExecuteAddAppAsync(App newApp)
    {
        ValidateApp(app: newApp, parameterName: "inputApp");

        if (string.IsNullOrEmpty(value: newApp.DefaultTheme))
        {
            newApp.DefaultTheme = "Default";
        }

        newApp.Cultures = BuildCulturesForApp(newApp: newApp);
        newApp.Roles = BuildRolesForApp(app: newApp, isFirstApp: out bool isFirstApp);
        App storedApp = await AddAppToStorageAsync(newApp: newApp, isFirstApp: isFirstApp);

        if (storedApp.Roles != null)
        {
            foreach (Role role in storedApp.Roles)
            {
                role.AppId = storedApp.Id;
                role.App = null;

                await AddRoleAsync(newRole: new Role
                {
                    Id = role.Id,
                    AppId = role.AppId,
                    Name = role.Name,
                    Description = role.Description,
                    Privs = role.Privs,
                });

                if (role.Users == null)
                {
                    continue;
                }

                foreach (UserRole user in role.Users)
                {
                    user.RoleId = role.Id;
                    user.Role = null;

                    await AddUserRoleAsync(newUserRole: new UserRole
                    {
                        RoleId = user.RoleId,
                        UserId = user.UserId,
                    });
                }
            }
        }

        StampAppChildren(app: storedApp);
        return storedApp;
    }

    private async ValueTask ExecuteDeleteAsync(int appId)
    {
        ValidateId(appId: appId, parameterName: "id");
        await DeleteAppFromStorageAsync(appId: appId);
    }

    private App ExecuteGetApp(int appId)
    {
        ValidateId(appId: appId, parameterName: "id");
        return GetAppFromStorage(appId: appId);
    }

    private App ExecuteGetByDomainApp(string domain, bool ignoreFilters = false)
    {
        ValidateDomain(domain: domain, parameterName: "domain");

        return GetAllAppsFromStorage(ignoreFilters: ignoreFilters)
            .Where(predicate: app => app.Domain == domain)
            .FirstOrDefault();
    }

    private async ValueTask<App> ExecuteUpdateAppAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        App existingApp = GetAppFromStorage(appId: app.Id, ignoreFilters: true);

        if (existingApp == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        existingApp.DefaultCultureId = app.DefaultCultureId;
        existingApp.TenantId = app.TenantId;
        existingApp.Name = app.Name;
        existingApp.Domain = app.Domain;
        existingApp.DefaultTheme = app.DefaultTheme;
        existingApp.ConfigJson = app.ConfigJson;
        existingApp.Cultures = app.Cultures ?? existingApp.Cultures;
        existingApp.Pages = app.Pages ?? existingApp.Pages;
        existingApp.Components = app.Components ?? existingApp.Components;
        existingApp.Scripts = app.Scripts ?? existingApp.Scripts;
        existingApp.Roles = app.Roles ?? existingApp.Roles;
        existingApp.Templates = app.Templates ?? existingApp.Templates;
        existingApp.Resources = app.Resources ?? existingApp.Resources;
        existingApp.Layouts = app.Layouts ?? existingApp.Layouts;

        if (app.Cultures != null)
        {
            existingApp.Cultures = BuildCulturesForApp(newApp: existingApp);
        }

        App updatedApp = await UpdateAppInStorageAsync(updatedApp: existingApp);

        if (updatedApp.Roles != null)
        {
            Role[] existingRoles = GetAllRolesIgnoringFilters()
                .Where(predicate: role => role.AppId == updatedApp.Id)
                .ToArray();

            foreach (Role role in updatedApp.Roles)
            {
                role.AppId = updatedApp.Id;
                role.App = null;

                if (existingRoles.Any(predicate: existingRole => existingRole.Id == role.Id))
                {
                    await UpdateRoleAsync(updatedRole: new Role
                    {
                        Id = role.Id,
                        AppId = role.AppId,
                        Name = role.Name,
                        Description = role.Description,
                        Privs = role.Privs,
                    });
                }
                else
                {
                    await AddRoleAsync(newRole: new Role
                    {
                        Id = role.Id,
                        AppId = role.AppId,
                        Name = role.Name,
                        Description = role.Description,
                        Privs = role.Privs,
                    });
                }

                UserRole[] existingUserRoles = GetAllUserRolesIgnoringFilters()
                    .Where(predicate: userRole => userRole.RoleId == role.Id)
                    .ToArray();

                string[] incomingUserIds = (role.Users ?? Array.Empty<UserRole>())
                    .Select(selector: userRole => userRole.UserId)
                    .Where(predicate: userId => !string.IsNullOrWhiteSpace(value: userId))
                    .Distinct(comparer: StringComparer.Ordinal)
                    .ToArray();

                UserRole[] userRolesToDelete = existingUserRoles
                    .Where(predicate: userRole => !incomingUserIds.Contains(value: userRole.UserId, comparer: StringComparer.Ordinal))
                    .ToArray();

                if (userRolesToDelete.Length > 0)
                {
                    await DeleteAllUserRolesAsync(deletedUserRole: userRolesToDelete);
                }

                foreach (string userId in incomingUserIds)
                {
                    if (existingUserRoles.Any(predicate: userRole => string.Equals(a: userRole.UserId, b: userId, comparisonType: StringComparison.Ordinal)))
                    {
                        continue;
                    }

                    await AddUserRoleAsync(newUserRole: new UserRole
                    {
                        RoleId = role.Id,
                        UserId = userId,
                    });
                }
            }
        }

        StampAppChildren(app: updatedApp);
        return updatedApp;
    }

    private async ValueTask<App> GetAppForRenderFromStorageAsync(int appId) =>
        (await service.GetAppForRenderAppOperationAsync(
            appOperation: new AppOperation { AppId = appId })).App;

    private App GetAppForDeleteFromStorage(int appId) =>
        service.GetAppForDeleteAppOperation(
            appOperation: new AppOperation { AppId = appId }).App;

    private App GetAppFromStorage(int appId, bool ignoreFilters = false)
    {
        AppOperation appOperation = new() { AppId = appId };

        App app = ignoreFilters
            ? service.GetUnfilteredAppAppOperation(appOperation: appOperation).App
            : service.GetVisibleAppAppOperation(appOperation: appOperation).App;

        if (app != null || ignoreFilters)
        {
            return app;
        }

        bool exists = service.GetUnfilteredAppAppOperation(
            appOperation: new AppOperation { AppId = appId }).App != null;

        return exists
            ? throw new SecurityException(message: "Access Denied!")
            : null;
    }

    private IQueryable<App> GetAllAppsFromStorage(bool ignoreFilters = false)
    {
        AppOperation appOperation = new();

        return ignoreFilters
            ? service.GetUnfilteredAppsAppOperation(appOperation: appOperation).Apps.AsQueryable()
            : service.GetVisibleAppsAppOperation(appOperation: appOperation).Apps.AsQueryable();
    }

    private async ValueTask<App> AddAppToStorageAsync(App newApp, bool isFirstApp)
    {
        if (!isFirstApp)
        {
            Authorize(appId: null, privilege: "App_create");
        }

        AppOperation appOperation = await service.AddAppOperationAsync(
            newAppOperation: new AppOperation { App = newApp });

        return appOperation.App;
    }

    private async ValueTask<App> UpdateAppInStorageAsync(App updatedApp)
    {
        Authorize(appId: updatedApp.Id, privilege: "App_update");

        AppOperation appOperation = await service.UpdateAppOperationAsync(
            updatedAppOperation: new AppOperation { App = updatedApp });

        return appOperation.App;
    }

    private async ValueTask DeleteAppFromStorageAsync(int appId)
    {
        App app = GetAppForDeleteFromStorage(appId: appId);

        if (app == null)
        {
            return;
        }

        if (app.Roles?.Any() == true)
        {
            Authorize(appId: app.Id, privilege: "App_delete");
        }

        await service.DeleteAppOperationAsync(
            deletedAppOperation: new AppOperation { App = app });
    }

    private string GetRequestPath() =>
        service.GetRequestPathAppOperation(appOperation: new AppOperation()).Text;

    private string GetRequestHost() =>
        service.GetRequestHostAppOperation(appOperation: new AppOperation()).Text;

    private IQueryable<Culture> GetAllCultures() =>
        service.GetCulturesAppOperation(appOperation: new AppOperation()).Cultures.AsQueryable();

    private IQueryable<Privilege> GetAllPrivileges() =>
        service.GetPrivilegesAppOperation(appOperation: new AppOperation()).Privileges.AsQueryable();

    private User GetCurrentUser()
    {
        string userId = service.GetCurrentUserAppOperation(
            appOperation: new AppOperation()).Text;

        return userId == null ? null : new User { Id = userId };
    }

    private string GetCurrentUserId() =>
        service.GetCurrentUserIdAppOperation(appOperation: new AppOperation()).Text;

    private bool IsAdminOfApp(int appId) =>
        service.IsAdminOfAppAppOperation(
            appOperation: new AppOperation { AppId = appId }).Result;

    private void Authorize(int? appId, string privilege) =>
        service.AuthorizeAppOperation(appOperation: new AppOperation
        {
            AppId = appId ?? 0,
            OptionalAppId = appId,
            Privilege = privilege
        });

    private async ValueTask<Role> AddRoleAsync(Role newRole) =>
        (await service.AddRoleAppOperationAsync(
            newAppOperation: new AppOperation { Role = newRole })).Role;

    private async ValueTask<Role> UpdateRoleAsync(Role updatedRole) =>
        (await service.UpdateRoleAppOperationAsync(
            updatedAppOperation: new AppOperation { Role = updatedRole })).Role;

    private IQueryable<Role> GetAllRolesIgnoringFilters() =>
        service.GetRolesAppOperation(appOperation: new AppOperation()).Roles.AsQueryable();

    private async ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole) =>
        (await service.AddUserRoleAppOperationAsync(
            newAppOperation: new AppOperation { UserRole = newUserRole })).UserRole;

    private IQueryable<UserRole> GetAllUserRolesIgnoringFilters() =>
        service.GetUserRolesAppOperation(appOperation: new AppOperation()).UserRoles.AsQueryable();

    private ValueTask<AppOperation> DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRole) =>
        service.DeleteUserRolesAppOperationAsync(
            deletedAppOperation: new AppOperation { DeletedUserRoles = deletedUserRole.ToArray() });

    private IQueryable<Page> GetAllPagesIgnoringFilters() =>
        service.GetPagesAppOperation(appOperation: new AppOperation()).Pages.AsQueryable();

    private async ValueTask<Page> UpdatePageAsync(Page updatedPage) =>
        (await service.UpdatePageAppOperationAsync(
            updatedAppOperation: new AppOperation { Page = updatedPage })).Page;
}