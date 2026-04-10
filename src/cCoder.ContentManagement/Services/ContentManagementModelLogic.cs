using System.Security;
using Content = cCoder.Data.Models.CMS.Content;
using Page = cCoder.Data.Models.CMS.Page;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services;

internal static class ContentManagementModelLogic
{
    internal static bool Can(User user, int? appId, string operation)
    {
        operation = operation?.ToLowerInvariant() ?? string.Empty;

        return user != null
            && ((appId.HasValue && IsAdminOfApp(user, appId.Value))
                || (user.Roles?.Any(role =>
                    (!appId.HasValue || role.Role?.AppId == appId.Value)
                    && (role.Role?.Privileges?.Contains(operation) ?? false)) ?? false));
    }

    internal static bool IsAdminOfApp(User user, int appId) =>
        user?.Roles?.Any(role => role.Role?.AppId == appId && (role.Role?.Privileges?.Contains("app_admin") ?? false)) ?? false;

    internal static bool UserCan(Page page, User user, string privilege)
    {
        Guid[] userRoles = user?.Roles?.Select(role => role.RoleId).ToArray() ?? [];

        return IsAdminOfApp(user, page.AppId)
            || (page.Roles?.Where(pageRole => userRoles.Contains(pageRole.RoleId))
                    .SelectMany(pageRole => pageRole.Role?.Privileges ?? [])
                    .Contains(privilege?.ToLowerInvariant() ?? string.Empty) ?? false);
    }

    internal static string Title(Page page, string culture) =>
        InfoForCulture(page, culture).Title ?? string.Empty;

    internal static string Description(Page page, string culture) =>
        InfoForCulture(page, culture).Description ?? string.Empty;

    internal static string Keywords(Page page, string culture) =>
        InfoForCulture(page, culture).Keywords ?? string.Empty;

    internal static PageInfo InfoForCulture(Page page, string culture)
    {
        culture ??= string.Empty;

        if (page?.PageInfo == null || !page.PageInfo.Any())
        {
            return new PageInfo
            {
                CultureId = culture,
                Title = page?.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
        }

        IOrderedEnumerable<PageInfo> orderedInfo = page.PageInfo
            .OrderByDescending(info => info.CultureId?.Length ?? 0);

        return orderedInfo.FirstOrDefault(info => culture == info.CultureId || culture.Contains(info.CultureId ?? string.Empty))
            ?? orderedInfo.FirstOrDefault()
            ?? new PageInfo
            {
                CultureId = culture,
                Title = page.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
    }

    internal static Content ContentForCulture(Page page, string name, string culture)
    {
        culture ??= string.Empty;

        Content result = page?.Contents?
            .Where(content => (content.CultureId?.Length ?? 0) <= culture.Length)
            .OrderByDescending(content => content.CultureId?.Length ?? 0)
            .FirstOrDefault(content => content.Name == name && culture.Contains(content.CultureId ?? string.Empty));

        result ??= page?.Contents?.FirstOrDefault(content => content.Name == name && string.IsNullOrEmpty(content.CultureId));

        return result ?? new Content
        {
            CultureId = string.Empty,
            Name = name,
            Html = string.Empty
        };
    }

    internal static void ThrowIfNoAccess(bool hasAccess)
    {
        if (!hasAccess)
        {
            throw new SecurityException("Access Denied!");
        }
    }
}
