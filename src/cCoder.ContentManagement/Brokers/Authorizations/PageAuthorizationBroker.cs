// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.ContentManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Authorizations;

internal sealed class PageAuthorizationBroker(
    ICoreContextFactory coreContextFactory) : IPageAuthorizationBroker
{
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

    public async ValueTask<PageAuthorizationResult> GetAuthorizedPageAsync(
        string domain,
        string path)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        return await context.Apps
            .Where(predicate: app => app.Domain == domain)
            .Select(selector: app => new PageAuthorizationResult
            {
                PageId = app.Pages
                    .Where(predicate: page => page.Path == path)
                    .Select(selector: page => (int?)page.Id)
                    .SingleOrDefault(),
                Layout = app.Pages
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
    }

    public async ValueTask<PageAuthorizationResult> GetPageIgnoringFiltersAsync(
        string domain,
        string path)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        return await context.Apps
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
    }
}