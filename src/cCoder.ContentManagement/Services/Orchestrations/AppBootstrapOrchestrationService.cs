// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class AppBootstrapOrchestrationService(
    ICultureService cultureService,
    IPrivilegeService privilegeService,
    IAuthorizationService authorizationService)
        : IAppBootstrapOrchestrationService
{
    public App PrepareNewApp(App app, bool isFirstApp) =>
        TryCatch(operation: () =>
    {
        ValidateNewAppOnPrepare(inputs: [app, isFirstApp]);
        ArgumentNullException.ThrowIfNull(argument: app);

        if (string.IsNullOrEmpty(value: app.DefaultTheme))
        {
            app.DefaultTheme = "Default";
        }

        app.Cultures = BuildCulturesForApp(app: app);
        app.Roles = BuildRolesForApp(app: app, isFirstApp: isFirstApp);
        return app;
    });

    public void StampAppChildren(App app) =>
        TryCatch(operation: () =>
    {
        ValidateAppChildrenOnStamp(inputs: [app]);
        ArgumentNullException.ThrowIfNull(argument: app);

        StampAppIds(appId: app.Id, items: app.Cultures);
        StampAppIds(appId: app.Id, items: app.Pages);
        StampAppIds(appId: app.Id, items: app.Components);
        StampAppIds(appId: app.Id, items: app.Scripts);
        StampAppIds(appId: app.Id, items: app.Templates);
        StampAppIds(appId: app.Id, items: app.Resources);
        StampAppIds(appId: app.Id, items: app.Layouts);

        foreach (Role role in app.Roles ?? [])
        {
            role.AppId = app.Id;
            role.App = null;

            foreach (UserRole userRole in role.Users ?? [])
            {
                userRole.RoleId = role.Id;
                userRole.Role = null;
            }
        }
    });

    private ICollection<AppCulture> BuildCulturesForApp(App app)
    {
        IEnumerable<string> requestedCultures = app.Cultures?
            .Select(selector: culture => culture.CultureId ?? string.Empty)
            ?? [];

        string[] requestedCultureIds = requestedCultures
            .Distinct(comparer: StringComparer.Ordinal)
            .ToArray();

        HashSet<string> requestedCultureSet = requestedCultureIds
            .ToHashSet(comparer: StringComparer.Ordinal);

        AppCulture[] cultures = cultureService.GetAllCulture()
            .Where(predicate: culture =>
                culture.Id == string.Empty
                || requestedCultureSet.Contains(item: culture.Id))
            .Select(selector: culture => new AppCulture
            {
                CultureId = culture.Id
            })
            .ToArray();

        if (string.IsNullOrEmpty(value: app.DefaultCultureId))
        {
            app.DefaultCultureId = requestedCultureIds.FirstOrDefault()
                ?? string.Empty;
        }

        return cultures;
    }

    private ICollection<Role> BuildRolesForApp(App app, bool isFirstApp)
    {
        List<Role> roles = (app.Roles ?? []).ToList();

        string currentUserId = authorizationService.GetCurrentUser().User?.Id
            ?? authorizationService.GetCurrentUserId();

        string defaultUserId = string.IsNullOrWhiteSpace(value: currentUserId)
            ? "Guest"
            : currentUserId;

        string bootstrapUserId = isFirstApp
            ? NormalizeBootstrapUserId(userId: currentUserId)
            : defaultUserId;

        Privilege[] privileges = privilegeService
            .GetAllPrivileges()
            .ToArray();

        string[] administratorPrivilegeIds = privileges
            .Where(predicate: privilege =>
                isFirstApp || privilege.Id != "app_create")
            .Select(selector: privilege => privilege.Id)
            .ToArray();

        string[] userPrivilegeIds = privileges
            .Where(predicate: privilege =>
                string.Equals(
                    a: privilege.Operation,
                    b: "Read",
                    comparisonType: StringComparison.OrdinalIgnoreCase)
                && !privilege.Type.StartsWith(
                    value: "Flow",
                    comparisonType: StringComparison.OrdinalIgnoreCase)
                && !privilege.Type.StartsWith(
                    value: "Workflow",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
            .Select(selector: privilege => privilege.Id)
            .ToArray();

        EnsureRole(
            roles: roles,
            roleName: "Administrators",
            requiredPrivileges: administratorPrivilegeIds,
            userId: bootstrapUserId);

        EnsureRole(
            roles: roles,
            roleName: "Users",
            requiredPrivileges: userPrivilegeIds,
            userId: bootstrapUserId);

        EnsureRole(
            roles: roles,
            roleName: "Guests",
            requiredPrivileges: userPrivilegeIds,
            userId: "Guest");

        if (isFirstApp)
        {
            EnsureRole(
                roles: roles,
                roleName: "System Admins",
                requiredPrivileges: ["app_create"],
                userId: bootstrapUserId);
        }

        return roles;
    }

    private static void EnsureRole(
        ICollection<Role> roles,
        string roleName,
        IEnumerable<string> requiredPrivileges,
        string userId)
    {
        Role role = roles.FirstOrDefault(predicate: foundRole =>
            string.Equals(
                a: foundRole.Name,
                b: roleName,
                comparisonType: StringComparison.OrdinalIgnoreCase));

        if (role == null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Users = [],
                Pages = [],
                Privileges = []
            };

            roles.Add(item: role);
        }

        role.Users ??= [];
        role.Pages ??= [];

        role.Privileges = role.Privileges
            .Union(second: requiredPrivileges, comparer: StringComparer.OrdinalIgnoreCase)
            .ToList();

        role.Privs = string.Join(separator: ',', values: role.Privileges);

        if (!string.IsNullOrWhiteSpace(value: userId)
            && !role.Users.Any(predicate: existingUserRole =>
                existingUserRole.UserId == userId))
        {
            role.Users.Add(item: new UserRole
            {
                RoleId = role.Id,
                UserId = userId
            });
        }
    }

    private static string NormalizeBootstrapUserId(string userId) =>
        string.IsNullOrWhiteSpace(value: userId)
        || string.Equals(
            a: userId,
            b: "Guest",
            comparisonType: StringComparison.OrdinalIgnoreCase)
            ? null
            : userId;

    private static void StampAppIds(
        int appId,
        IEnumerable<AppCulture> items)
    {
        foreach (AppCulture item in items ?? [])
        {
            item.AppId = appId;
        }
    }

    private static void StampAppIds(int appId, IEnumerable<Page> items)
    {
        foreach (Page item in items ?? [])
        {
            item.AppId = appId;
        }
    }

    private static void StampAppIds(
        int appId,
        IEnumerable<Component> items)
    {
        foreach (Component item in items ?? [])
        {
            item.AppId = appId;
        }
    }

    private static void StampAppIds(int appId, IEnumerable<Script> items)
    {
        foreach (Script item in items ?? [])
        {
            item.AppId = appId;
        }
    }

    private static void StampAppIds(int appId, IEnumerable<Template> items)
    {
        foreach (Template item in items ?? [])
        {
            item.AppId = appId;
        }
    }

    private static void StampAppIds(int appId, IEnumerable<Resource> items)
    {
        foreach (Resource item in items ?? [])
        {
            item.AppId = appId;
        }
    }

    private static void StampAppIds(int appId, IEnumerable<Layout> items)
    {
        foreach (Layout item in items ?? [])
        {
            item.AppId = appId;
        }
    }
}