// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services;

internal static class ContentManagementModelLogic
{
    internal static bool Can(User user, int? appId, string operation)
    {
        operation = operation?.ToLowerInvariant() ?? string.Empty;

        return user != null
            && ((appId.HasValue && IsAdminOfApp(user: user, appId: appId.Value))
                || (user.Roles?.Any(predicate: role =>
                    (!appId.HasValue || role.Role?.AppId == appId.Value)
                    && (role.Role?.Privileges?.Contains(item: operation) ?? false)) ?? false));
    }

    internal static bool IsAdminOfApp(User user, int appId) =>
        user?.Roles?.Any(predicate: role => role.Role?.AppId == appId && (role.Role?.Privileges?.Contains(item: "app_admin") ?? false)) ?? false;

    internal static bool UserCan(Page page, User user, string privilege)
    {
        Guid[] userRoles = user?.Roles?.Select(selector: role => role.RoleId)
            .ToArray() ?? [];

        return IsAdminOfApp(user: user, appId: page.AppId)
            || (page.Roles?.Where(predicate: pageRole => userRoles.Contains(value: pageRole.RoleId))
            .SelectMany(selector: pageRole => pageRole.Role?.Privileges ?? [])
            .Contains(value: privilege?.ToLowerInvariant() ?? string.Empty) ?? false);
    }

    internal static string Title(Page page, string culture) =>
        InfoForCulture(page: page, culture: culture)
        .Title ?? string.Empty;

    internal static string Description(Page page, string culture) =>
        InfoForCulture(page: page, culture: culture)
        .Description ?? string.Empty;

    internal static string Keywords(Page page, string culture) =>
        InfoForCulture(page: page, culture: culture)
        .Keywords ?? string.Empty;

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
            .OrderByDescending(keySelector: info => info.CultureId?.Length ?? 0);

        return orderedInfo.FirstOrDefault(predicate: info => culture == info.CultureId || culture.Contains(value: info.CultureId ?? string.Empty))
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
            .Where(predicate: content => (content.CultureId?.Length ?? 0) <= culture.Length)
            .OrderByDescending(keySelector: content => content.CultureId?.Length ?? 0)
            .FirstOrDefault(predicate: content => content.Name == name && culture.Contains(value: content.CultureId ?? string.Empty));

        result ??= page?.Contents?.FirstOrDefault(predicate: content => content.Name == name && string.IsNullOrEmpty(value: content.CultureId));

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
            throw new SecurityException(message: "Access Denied!");
        }
    }
}