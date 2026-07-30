// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Models;

namespace cCoder.Core.Services.Tests;

internal static class TestUsers
{
    internal static User WithPrivilege(string privilege, int appId = 1) =>
        WithPrivileges(privileges: [privilege], appId: appId);

    internal static User WithPrivileges(IEnumerable<string> privileges, int appId = 1)
    {
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = "Test Role",
            Privs = string.Join(separator: ',', values: privileges.Select(selector: p => p.ToLowerInvariant())),
        };

        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
        };

        UserRole userRole = new()
        {
            Role = role,
            RoleId = role.Id,
            User = user,
            UserId = user.Id,
        };

        user.Roles = [userRole];
        role.Users = [userRole];

        role.App = new App
        {
            Id = appId,
            Name = "App",
            Domain = "app.local",
        };

        return user;
    }

    internal static User WithoutPrivileges() =>
        new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = [],
        };

    internal static bool UserCanPage(PageAuthorization authorization)
    {
        Guid[] userRoles = authorization.User?.Roles?
            .Select(selector: role => role.RoleId)
            .ToArray() ?? [];

        bool isAppAdmin = authorization.User?.Roles?.Any(predicate: role =>
            role.Role?.AppId == authorization.Page.AppId
            && (role.Role.Privileges?.Contains(value: "app_admin") ?? false))
            ?? false;

        return isAppAdmin
            || (authorization.Page.Roles?
                .Where(predicate: pageRole =>
                    userRoles.Contains(value: pageRole.RoleId))
                .SelectMany(selector: pageRole =>
                    pageRole.Role?.Privileges ?? [])
                .Contains(value:
                    authorization.Privilege?.ToLowerInvariant()
                    ?? string.Empty) ?? false);
    }
}