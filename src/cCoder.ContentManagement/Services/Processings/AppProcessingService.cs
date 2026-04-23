using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Foundations.Storages;
using App = cCoder.Data.Models.CMS.App;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;
using Component = cCoder.Data.Models.CMS.Component;
using Layout = cCoder.Data.Models.CMS.Layout;
using Page = cCoder.Data.Models.CMS.Page;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;
using Template = cCoder.Data.Models.CMS.Template;
using Role = cCoder.Data.Models.Security.Role;
using PageRole = cCoder.Data.Models.Security.PageRole;
using User = cCoder.Data.Models.Security.User;
using UserRole = cCoder.Data.Models.Security.UserRole;

namespace cCoder.ContentManagement.Services.Processings;

internal class AppProcessingService(
    IAppService service,
    ICultureService cultureService,
    IPrivilegeBroker privilegeBroker,
    IAppEventProcessingService appEventProcessingService,
    IAuthorizationBroker authorizationBroker,
    IRoleBroker roleBroker,
    IUserRoleBroker userRoleBroker,
    HttpContext httpContext = null) : IAppProcessingService
{
    public App Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public string GetDomain(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        return service.GetAll(ignoreFilters)
            .Where(app => app.Id == id)
            .Select(app => app.Domain)
            .FirstOrDefault();
    }

    public App GetByDomain(string domain, bool ignoreFilters = false)
    {
        ValidateDomain(domain, "domain");
        return service.GetAll(ignoreFilters)
            .Where(app => app.Domain == domain)
            .FirstOrDefault();
    }

    public IQueryable<App> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public async ValueTask<App> AddAsync(App inputApp)
    {
        ValidateApp(inputApp, "inputApp");

        if (string.IsNullOrEmpty(inputApp.DefaultTheme))
        {
            inputApp.DefaultTheme = "Default";
        }

        inputApp.Cultures = BuildCulturesForApp(inputApp);
        inputApp.Roles = BuildRolesForApp(inputApp);
        App storedApp = await service.AddAsync(inputApp);
        if (storedApp.Roles != null)
        {
            foreach (Role role in storedApp.Roles)
            {
                role.AppId = storedApp.Id;
                role.App = null;

                await roleBroker.AddRoleAsync(new Role
                {
                    Id = role.Id,
                    AppId = role.AppId,
                    Name = role.Name,
                    Description = role.Description,
                    Privs = role.Privs,
                });

                foreach (UserRole user in role.Users ?? Array.Empty<UserRole>())
                {
                    user.RoleId = role.Id;
                    user.Role = role;

                    await userRoleBroker.AddUserRoleAsync(new UserRole
                    {
                        RoleId = user.RoleId,
                        UserId = user.UserId,
                    });
                }
            }
        }

        StampAppChildren(storedApp);
        await appEventProcessingService.RaiseAppAddEventAsync(storedApp);
        return storedApp;
    }

    public async ValueTask<App> UpdateAsync(App app)
    {
        ValidateApp(app, "app");
        App existingApp = service.Get(app.Id, ignoreFilters: true);
        if (existingApp == null)
        {
            throw new SecurityException("Access Denied!");
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
            existingApp.Cultures = BuildCulturesForApp(existingApp);
        }

        App updatedApp = await service.UpdateAsync(existingApp);
        if (updatedApp.Roles != null)
        {
            Role[] existingRoles = roleBroker.GetAllRoles(ignoreFilters: true)
                .Where(role => role.AppId == updatedApp.Id)
                .ToArray();

            foreach (Role role in updatedApp.Roles)
            {
                role.AppId = updatedApp.Id;
                role.App = null;

                if (existingRoles.Any(existingRole => existingRole.Id == role.Id))
                {
                    await roleBroker.UpdateRoleAsync(new Role
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
                    await roleBroker.AddRoleAsync(new Role
                    {
                        Id = role.Id,
                        AppId = role.AppId,
                        Name = role.Name,
                        Description = role.Description,
                        Privs = role.Privs,
                    });
                }

                UserRole[] existingUserRoles = userRoleBroker.GetAllUserRoles(ignoreFilters: true)
                    .Where(userRole => userRole.RoleId == role.Id)
                    .ToArray();

                string[] incomingUserIds = (role.Users ?? Array.Empty<UserRole>())
                    .Select(userRole => userRole.UserId)
                    .Where(userId => !string.IsNullOrWhiteSpace(userId))
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();

                UserRole[] userRolesToDelete = existingUserRoles
                    .Where(userRole => !incomingUserIds.Contains(userRole.UserId, StringComparer.Ordinal))
                    .ToArray();

                if (userRolesToDelete.Length > 0)
                {
                    await userRoleBroker.DeleteAllUserRolesAsync(userRolesToDelete);
                }

                foreach (string userId in incomingUserIds)
                {
                    if (existingUserRoles.Any(userRole => string.Equals(userRole.UserId, userId, StringComparison.Ordinal)))
                    {
                        continue;
                    }

                    await userRoleBroker.AddUserRoleAsync(new UserRole
                    {
                        RoleId = role.Id,
                        UserId = userId,
                    });
                }
            }
        }

        StampAppChildren(updatedApp);
        await appEventProcessingService.RaiseAppUpdateEventAsync(updatedApp);
        return updatedApp;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        await service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<App>>> AddOrUpdate(IEnumerable<App> items)
    {
        ValidateApps(items, "items");
        List<cCoder.ContentManagement.Models.Result<App>> results = [];

        foreach (App item in items)
        {
            try
            {
                App app = item.Id < 1
                    ? await AddAsync(item)
                    : await UpdateAsync(item);

                results.Add(new cCoder.ContentManagement.Models.Result<App>
                {
                    Success = true,
                    Item = app,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<App>
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
        ValidateApps(items, "items");
        foreach (App item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    public IQueryable<User> GetAppUsers(int appId)
    {
        ValidateId(appId, "appId");
        App app = Get(appId);
        if (app != null)
        {
            return app.Roles.SelectMany((Role role) => role.Users.Select((UserRole userRole) => userRole.User)).AsQueryable();
        }
        throw new SecurityException("Access Denied!");
    }

    public App ResolveCurrentApp()
    {
        string text = httpContext?.Request.Path.Value ?? string.Empty;
        if (text.Contains("/webdav", StringComparison.OrdinalIgnoreCase) && text.Contains("Core/App(", StringComparison.OrdinalIgnoreCase))
        {
            int num = text.IndexOf("Core/App(", StringComparison.OrdinalIgnoreCase) + 9;
            int num2 = text.IndexOf(')', num);
            if (num2 > num)
            {
                int num3 = num;
                if (int.TryParse(text.Substring(num3, num2 - num3), out var result))
                {
                    return service.Get(result);
                }
            }
        }
        string domain = httpContext?.Request.Host.Host ?? string.Empty;
        return GetByDomain(domain);
    }

    public async ValueTask UpdatePageOrderAsync(int key, App app)
    {
        ValidateId(key, "key");
        ValidateApp(app, "app");
        await service.UpdatePageOrderAsync(key, app.Pages ?? new List<Page>());
    }

    private ICollection<AppCulture> BuildCulturesForApp(App newApp)
    {
        IEnumerable<string> enumerable = newApp.Cultures?.Select((AppCulture culture) => culture.CultureId) ?? Array.Empty<string>();
        string[] requestedCultureIds = enumerable.Distinct().ToArray();
        AppCulture[] culturesForApp = cultureService.GetAll(ignoreFilters: false)
            .Where(culture => culture.Id == string.Empty || requestedCultureIds.Contains(culture.Id))
            .Select(culture => new AppCulture
            {
                CultureId = culture.Id
            })
            .ToArray();
        if (string.IsNullOrEmpty(newApp.DefaultCultureId))
        {
            newApp.DefaultCultureId = enumerable.FirstOrDefault() ?? string.Empty;
        }
        return culturesForApp;
    }

    private ICollection<Role> BuildRolesForApp(App app)
    {
        List<Role> list = (app.Roles ?? new List<Role>()).ToList();
        string currentUserId = authorizationBroker.GetCurrentUser()?.Id;
        bool isFirstApp = !service.GetAll(ignoreFilters: true).Any();
        string defaultUserId = string.IsNullOrWhiteSpace(currentUserId) ? "Guest" : currentUserId;
        string bootstrapUserId = isFirstApp
            ? NormalizeBootstrapUserId(currentUserId)
            : defaultUserId;

        string[] administratorPrivilegeIds = privilegeBroker.GetAllPrivileges(ignoreFilters: false)
            .ToArray()
            .Where(privilege => isFirstApp || privilege.Id != "app_create")
            .Select(privilege => privilege.Id)
            .ToArray();

        string[] userPrivilegeIds = privilegeBroker.GetAllPrivileges(ignoreFilters: false)
            .ToArray()
            .Where(privilege =>
                string.Equals(privilege.Operation, "Read", StringComparison.OrdinalIgnoreCase) &&
                !privilege.Type.StartsWith("Flow", StringComparison.OrdinalIgnoreCase) &&
                !privilege.Type.StartsWith("Workflow", StringComparison.OrdinalIgnoreCase))
            .Select(privilege => privilege.Id)
            .ToArray();

        EnsureRole(list, "Administrators", administratorPrivilegeIds, bootstrapUserId);
        EnsureRole(list, "Users", userPrivilegeIds, bootstrapUserId);
        EnsureRole(list, "Guests", userPrivilegeIds, "Guest");

        foreach (Role item in list)
        {
            item.App = app;
            item.AppId = app.Id;
            Role role = item;
            if (role.Users == null)
            {
                ICollection<UserRole> collection = (role.Users = new List<UserRole>());
            }
            foreach (UserRole user in item.Users)
            {
                user.RoleId = item.Id;
                user.Role = item;
            }
        }
        return list;
    }

    private static void EnsureRole(ICollection<Role> roles, string roleName, IEnumerable<string> requiredPrivileges, string userId)
    {
        Role role = roles.FirstOrDefault((Role foundRole) => string.Equals(foundRole.Name, roleName, StringComparison.OrdinalIgnoreCase));
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
            roles.Add(role);
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
        list.AddRange(role.Privileges.Union<string>(requiredPrivileges, StringComparer.OrdinalIgnoreCase));
        role3.Privileges = list;
        role.Privs = string.Join(',', role.Privileges);
        if (!string.IsNullOrWhiteSpace(userId) && !role.Users.Any((UserRole existingUserRole) => existingUserRole.UserId == userId))
        {
            role.Users.Add(new UserRole
            {
                RoleId = role.Id,
                UserId = userId,
                Role = role
            });
        }
    }

    private static string NormalizeBootstrapUserId(string userId) =>
        string.IsNullOrWhiteSpace(userId) || string.Equals(userId, "Guest", StringComparison.OrdinalIgnoreCase)
            ? null
            : userId;

    private static void StampAppChildren(App app)
    {
        foreach (AppCulture culture in app.Cultures ?? Array.Empty<AppCulture>())
        {
            culture.AppId = app.Id;
        }

        foreach (Page page in app.Pages ?? Array.Empty<Page>())
        {
            page.AppId = app.Id;
        }

        foreach (Component component in app.Components ?? Array.Empty<Component>())
        {
            component.AppId = app.Id;
        }

        foreach (Script script in app.Scripts ?? Array.Empty<Script>())
        {
            script.AppId = app.Id;
        }

        foreach (Role role in app.Roles ?? Array.Empty<Role>())
        {
            role.AppId = app.Id;
            foreach (UserRole user in role.Users ?? Array.Empty<UserRole>())
            {
                user.RoleId = role.Id;
                user.Role = role;
            }
        }

        foreach (Template template in app.Templates ?? Array.Empty<Template>())
        {
            template.AppId = app.Id;
        }

        foreach (Resource resource in app.Resources ?? Array.Empty<Resource>())
        {
            resource.AppId = app.Id;
        }

        foreach (Layout layout in app.Layouts ?? Array.Empty<Layout>())
        {
            layout.AppId = app.Id;
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (string.IsNullOrWhiteSpace(app.Name))
        {
            throw new ValidationException(parameterName + ".Name is required.");
        }
        if (string.IsNullOrWhiteSpace(app.Domain))
        {
            throw new ValidationException(parameterName + ".Domain is required.");
        }
    }

    private static void ValidateApps(IEnumerable<App> apps, string parameterName)
    {
        if (apps == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateDomain(string domain, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
