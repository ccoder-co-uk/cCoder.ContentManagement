// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class AppProcessingService(
    IAppService service,
    ICultureService cultureService,
    IPrivilegeBroker privilegeBroker,
    IAuthorizationBroker authorizationBroker,
    IRoleBroker roleBroker,
    IUserRoleBroker userRoleBroker,
    HttpContext httpContext = null) : IAppProcessingService
{
    public App Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public string GetDomain(int id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        return service.GetAll(ignoreFilters: ignoreFilters)
            .Where(predicate: app => app.Id == id)
            .Select(selector: app => app.Domain)
            .FirstOrDefault();
    }

    public App GetByDomain(string domain, bool ignoreFilters = false)
    {
        ValidateDomain(domain: domain, parameterName: "domain");

        return service.GetAll(ignoreFilters: ignoreFilters)
            .Where(predicate: app => app.Domain == domain)
            .FirstOrDefault();
    }

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<App> AddAsync(App inputApp)
    {
        ValidateApp(app: inputApp, parameterName: "inputApp");

        if (string.IsNullOrEmpty(value: inputApp.DefaultTheme))
        {
            inputApp.DefaultTheme = "Default";
        }

        inputApp.Cultures = BuildCulturesForApp(newApp: inputApp);
        inputApp.Roles = BuildRolesForApp(app: inputApp);
        App storedApp = await service.AddAsync(app: inputApp);

        if (storedApp.Roles != null)
        {
            foreach (Role role in storedApp.Roles)
            {
                role.AppId = storedApp.Id;
                role.App = null;

                await roleBroker.AddRoleAsync(entity: new Role
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

                    await userRoleBroker.AddUserRoleAsync(entity: new UserRole
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

    public async ValueTask<App> UpdateAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        App existingApp = service.Get(id: app.Id, ignoreFilters: true);

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

        App updatedApp = await service.UpdateAsync(app: existingApp);

        if (updatedApp.Roles != null)
        {
            Role[] existingRoles = roleBroker.GetAllRoles(ignoreFilters: true)
                .Where(predicate: role => role.AppId == updatedApp.Id)
                .ToArray();

            foreach (Role role in updatedApp.Roles)
            {
                role.AppId = updatedApp.Id;
                role.App = null;

                if (existingRoles.Any(predicate: existingRole => existingRole.Id == role.Id))
                {
                    await roleBroker.UpdateRoleAsync(entity: new Role
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
                    await roleBroker.AddRoleAsync(entity: new Role
                    {
                        Id = role.Id,
                        AppId = role.AppId,
                        Name = role.Name,
                        Description = role.Description,
                        Privs = role.Privs,
                    });
                }

                UserRole[] existingUserRoles = userRoleBroker.GetAllUserRoles(ignoreFilters: true)
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
                    await userRoleBroker.DeleteAllUserRolesAsync(items: userRolesToDelete);
                }

                foreach (string userId in incomingUserIds)
                {
                    if (existingUserRoles.Any(predicate: userRole => string.Equals(a: userRole.UserId, b: userId, comparisonType: StringComparison.Ordinal)))
                    {
                        continue;
                    }

                    await userRoleBroker.AddUserRoleAsync(entity: new UserRole
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

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        await service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<App>>> AddOrUpdate(IEnumerable<App> items)
    {
        ValidateApps(apps: items, parameterName: "items");
        List<Result<App>> results = [];

        foreach (App item in items)
        {
            try
            {
                App app = item.Id < 1
                    ? await AddAsync(inputApp: item)
                    : await UpdateAsync(app: item);

                results.Add(item: new Result<App>
                {
                    Success = true,
                    Item = app,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<App>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<App> items)
    {
        ValidateApps(apps: items, parameterName: "items");

        foreach (App item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    public IQueryable<User> GetAppUsers(int appId)
    {
        ValidateId(id: appId, parameterName: "appId");
        App app = Get(id: appId);

        if (app != null)
        {
            return app.Roles.SelectMany(selector: (Role role) => role.Users.Select(selector: (UserRole userRole) => userRole.User))
                        .AsQueryable();
        }

        throw new SecurityException(message: "Access Denied!");
    }

    public App ResolveCurrentApp()
    {
        string text = httpContext?.Request.Path.Value ?? string.Empty;

        if (text.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase) && text.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            int num = text.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
            int num2 = text.IndexOf(value: ')', startIndex: num);

            if (num2 > num)
            {
                int num3 = num;

                if (int.TryParse(s: text.Substring(startIndex: num3, length: num2 - num3), result: out var result))
                {
                    return service.Get(id: result);
                }
            }
        }

        string domain = httpContext?.Request.Host.Host ?? string.Empty;
        return GetByDomain(domain: domain);
    }

    public async ValueTask UpdatePageOrderAsync(int key, App app)
    {
        ValidateId(id: key, parameterName: "key");
        ValidateApp(app: app, parameterName: "app");
        await service.UpdatePageOrderAsync(id: key, pages: app.Pages ?? new List<Page>());
    }

    private ICollection<AppCulture> BuildCulturesForApp(App newApp)
    {
        IEnumerable<string> enumerable = newApp.Cultures?.Select(selector: (AppCulture culture) => culture.CultureId) ?? Array.Empty<string>();

        string[] requestedCultureIds = enumerable.Distinct()
            .ToArray();

        AppCulture[] culturesForApp = cultureService.GetAll(ignoreFilters: false)
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

    private ICollection<Role> BuildRolesForApp(App app)
    {
        List<Role> list = (app.Roles ?? new List<Role>()).ToList();
        string currentUserId = authorizationBroker.GetCurrentUser()?.Id;

        bool isFirstApp = !service.GetAll(ignoreFilters: true)
            .Any();

        string defaultUserId = string.IsNullOrWhiteSpace(value: currentUserId) ? "Guest" : currentUserId;

        string bootstrapUserId = isFirstApp
            ? NormalizeBootstrapUserId(userId: currentUserId)
            : defaultUserId;

        string[] administratorPrivilegeIds = privilegeBroker.GetAllPrivileges(ignoreFilters: false)
            .ToArray()
            .Where(predicate: privilege => isFirstApp || privilege.Id != "app_create")
            .Select(selector: privilege => privilege.Id)
            .ToArray();

        string[] userPrivilegeIds = privilegeBroker.GetAllPrivileges(ignoreFilters: false)
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

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

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
}