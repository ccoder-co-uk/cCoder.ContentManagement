// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data;
using cCoder.ContentManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Authorizations;

internal sealed class PageAuthorizationBroker(
    ICoreContextFactory coreContextFactory)
        : IPageAuthorizationBroker
{
    public string GetCurrentUserId()
    {
        using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        return context.AuthInfo.SSOUserId;
    }

    public async ValueTask<bool> CanUpdatePageAsync(
        int appId,
        int pageId)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        string userId = context.AuthInfo.SSOUserId;

        return await context.Roles
            .IgnoreQueryFilters()
            .AnyAsync(predicate: role =>
                context.UserRoles
                    .IgnoreQueryFilters()
                    .Any(predicate: userRole =>
                        userRole.RoleId == role.Id
                        && userRole.UserId == userId)
                && ((role.AppId == appId
                        && role.Privs.Contains(value: "app_admin"))
                    || (role.Privs.Contains(value: "page_update")
                        && context.PageRoles
                            .IgnoreQueryFilters()
                            .Any(predicate: pageRole =>
                                pageRole.RoleId == role.Id
                                && pageRole.PageId == pageId))));
    }

    public async ValueTask<PageAuthorizationData> GetAuthorizedPageAsync(
        string domain,
        string path,
        string culture,
        string theme)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        string userId = new[] { context.AuthInfo.SSOUserId }
            .Where(predicate: value => !string.IsNullOrWhiteSpace(value: value))
            .DefaultIfEmpty(defaultValue: "Guest")
            .Single();

        PageAuthorizationResult result = await context.Apps
            .Where(predicate: app => app.Domain == domain)
            .Select(selector: app => new PageAuthorizationResult
            {
                PageId = app.Pages
                    .AsQueryable()
                    .IgnoreQueryFilters()
                    .Where(predicate: page => page.Path == path)
                    .Where(predicate: page =>
                        context.UserRoles
                            .IgnoreQueryFilters()
                            .Any(predicate: userRole =>
                                userRole.UserId == userId
                                && userRole.Role.AppId == app.Id
                                && userRole.Role.Privs.Contains(value: "app_admin"))
                        || context.PageRoles
                            .IgnoreQueryFilters()
                            .Any(predicate: pageRole =>
                                pageRole.PageId == page.Id
                                && pageRole.Role.Privs.Contains(value: "page_read")
                                && context.UserRoles
                                    .IgnoreQueryFilters()
                                    .Any(predicate: userRole =>
                                        userRole.UserId == userId
                                        && userRole.RoleId == pageRole.RoleId)))
                    .Select(selector: page => (int?)page.Id)
                    .SingleOrDefault(),
                Layout = app.Pages
                    .AsQueryable()
                    .IgnoreQueryFilters()
                    .Where(predicate: page => page.Path == path)
                    .Where(predicate: page =>
                        context.UserRoles
                            .IgnoreQueryFilters()
                            .Any(predicate: userRole =>
                                userRole.UserId == userId
                                && userRole.Role.AppId == app.Id
                                && userRole.Role.Privs.Contains(value: "app_admin"))
                        || context.PageRoles
                            .IgnoreQueryFilters()
                            .Any(predicate: pageRole =>
                                pageRole.PageId == page.Id
                                && pageRole.Role.Privs.Contains(value: "page_read")
                                && context.UserRoles
                                    .IgnoreQueryFilters()
                                    .Any(predicate: userRole =>
                                        userRole.UserId == userId
                                        && userRole.RoleId == pageRole.RoleId)))
                    .Select(selector: page => page.Layout)
                    .SingleOrDefault(),
                AppId = app.Id,
                TenantId = app.TenantId,
                Domain = app.Domain,
                DefaultCulture = (app.DefaultCultureId ?? string.Empty)
                    .Trim()
                    .ToLower(),
                DefaultTheme = (app.DefaultTheme ?? "Default")
                    .Trim(),
                AppConfigJson = app.ConfigJson,
                CacheCandidates = app.Pages
                    .AsQueryable()
                    .IgnoreQueryFilters()
                    .Where(predicate: page => page.Path == path)
                    .Where(predicate: page =>
                        context.UserRoles
                            .IgnoreQueryFilters()
                            .Any(predicate: userRole =>
                                userRole.UserId == userId
                                && userRole.Role.AppId == app.Id
                                && userRole.Role.Privs.Contains(value: "app_admin"))
                        || context.PageRoles
                            .IgnoreQueryFilters()
                            .Any(predicate: pageRole =>
                                pageRole.PageId == page.Id
                                && pageRole.Role.Privs.Contains(value: "page_read")
                                && context.UserRoles
                                    .IgnoreQueryFilters()
                                    .Any(predicate: userRole =>
                                        userRole.UserId == userId
                                        && userRole.RoleId == pageRole.RoleId)))
                    .SelectMany(selector: page => context.PageRenderCaches
                        .Where(predicate: cache =>
                            cache.PageId == page.Id
                            && (cache.Theme == theme
                                || (theme == string.Empty
                                    && cache.Theme == (app.DefaultTheme ?? "Default")
                                        .Trim()
                                        .ToLower()))))
                    .ToArray(),
                UserId = context.Users
                    .IgnoreQueryFilters()
                    .Where(predicate: user => user.Id == userId)
                    .Select(selector: user => user.Id)
                    .SingleOrDefault(),
                UserDefaultCultureId = context.Users
                    .IgnoreQueryFilters()
                    .Where(predicate: user => user.Id == userId)
                    .Select(selector: user => user.DefaultCultureId)
                    .SingleOrDefault(),
                UserDisplayName = context.Users
                    .IgnoreQueryFilters()
                    .Where(predicate: user => user.Id == userId)
                    .Select(selector: user => user.DisplayName)
                    .SingleOrDefault(),
                UserEmail = context.Users
                    .IgnoreQueryFilters()
                    .Where(predicate: user => user.Id == userId)
                    .Select(selector: user => user.Email)
                    .SingleOrDefault(),
                UserIsActive = context.Users
                    .IgnoreQueryFilters()
                    .Where(predicate: user => user.Id == userId)
                    .Select(selector: user => (bool?)user.IsActive)
                    .SingleOrDefault()
            })
            .SingleOrDefaultAsync();

        return new PageAuthorizationData
        {
            Result = result,
            CacheLookupCompleted = true
        };
    }

    public async ValueTask<PageAuthorizationData> GetPageIgnoringFiltersAsync(
        string domain,
        string path)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        PageAuthorizationResult result = await context.Apps
            .IgnoreQueryFilters()
            .Where(predicate: app => app.Domain == domain)
            .Select(selector: app => new PageAuthorizationResult
            {
                PageId = app.Pages
                    .AsQueryable()
                    .IgnoreQueryFilters()
                    .Where(predicate: page => page.Path == path)
                    .Select(selector: page => (int?)page.Id)
                    .SingleOrDefault(),
                Layout = app.Pages
                    .AsQueryable()
                    .IgnoreQueryFilters()
                    .Where(predicate: page => page.Path == path)
                    .Select(selector: page => page.Layout)
                    .SingleOrDefault(),
                AppId = app.Id,
                TenantId = app.TenantId,
                Domain = app.Domain,
                DefaultCulture = (app.DefaultCultureId ?? string.Empty)
                    .Trim()
                    .ToLower(),
                DefaultTheme = (app.DefaultTheme ?? "Default")
                    .Trim(),
                AppConfigJson = app.ConfigJson
            })
            .SingleOrDefaultAsync();

        return new PageAuthorizationData
        {
            Result = result,
            User = context.User
        };
    }

}