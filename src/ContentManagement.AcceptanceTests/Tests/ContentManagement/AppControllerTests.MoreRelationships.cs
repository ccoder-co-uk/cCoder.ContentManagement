// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Post_CreatesAppCultureRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string cultureId = await GetNonDefaultCultureIdAsync();

        // When

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "CultureApp"),
    domain = $"{Unique(prefix: "culture")}.local",
    defaultTheme = "Default",
    defaultCultureId = cultureId,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    cultures = new[] { new { cultureId } },
});

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);

        // Then

        actualChildren.Cultures.Select(selector: culture => culture.CultureId)
            .Should()
            .Contain(expected: [string.Empty, cultureId]);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Put_UpdatesAppCultureRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        IReadOnlyList<string> cultureIds = await GetNonDefaultCultureIdsAsync(count: 2);
        string originalCultureId = cultureIds[0];
        string updatedCultureId = cultureIds[1];

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "CultureApp"),
    domain = $"{Unique(prefix: "culture")}.local",
    defaultTheme = "Default",
    defaultCultureId = originalCultureId,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    cultures = new[] { new { cultureId = originalCultureId } },
});

        // When

        await UpdateAppAsync(
host: createdApp.Domain,
id: createdApp.Id,
payload: new
{
    id = createdApp.Id,
    name = createdApp.Name,
    domain = createdApp.Domain,
    defaultTheme = createdApp.DefaultTheme,
    defaultCultureId = updatedCultureId,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{\"updated\":true}",
    cultures = new[] { new { appId = createdApp.Id, cultureId = updatedCultureId } },
});

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);

        // Then

        actualChildren.Cultures.Select(selector: culture => culture.CultureId)
            .Should()
            .Contain(expected: [string.Empty, updatedCultureId]);

        actualChildren.Cultures.Select(selector: culture => culture.CultureId)
            .Should()
            .NotContain(unexpected: originalCultureId);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Delete_RemovesAppCultureRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string cultureId = await GetNonDefaultCultureIdAsync();

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "CultureApp"),
    domain = $"{Unique(prefix: "culture")}.local",
    defaultTheme = "Default",
    defaultCultureId = cultureId,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    cultures = new[] { new { cultureId } },
});

        // When
        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(appId: createdApp.Id);

        // Then

        actualCounts.AppExists.Should()
            .BeFalse();

        actualCounts.AppCultureCount.Should()
            .Be(expected: 0);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Post_CreatesComponentRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string componentName = Unique(prefix: "Component");

        // When

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "ComponentApp"),
    domain = $"{Unique(prefix: "component")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    components = new[]
                {
                    new
                    {
                        name = componentName,
                        description = "Acceptance component",
                        key = Unique(prefix: "component-key"),
                        resourceKey = "Default",
                        content = "<div>Component</div>",
                        script = string.Empty,
                    },
                },
});

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);

        // Then

        actualChildren.Components.Should()
            .ContainSingle(predicate: component => component.Name == componentName && component.AppId == createdApp.Id);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Put_UpdatesComponentRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "ComponentApp"),
    domain = $"{Unique(prefix: "component")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    components = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Component"),
                        description = "Original component",
                        key = Unique(prefix: "component-key"),
                        resourceKey = "Default",
                        content = "<div>Original</div>",
                        script = string.Empty,
                    },
                },
});

        Component originalComponent = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Components.Single();
        string updatedComponentName = Unique(prefix: "UpdatedComponent");

        // When

        await UpdateAppAsync(
host: createdApp.Domain,
id: createdApp.Id,
payload: new
{
    id = createdApp.Id,
    name = createdApp.Name,
    domain = createdApp.Domain,
    defaultTheme = createdApp.DefaultTheme,
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{\"updated\":true}",
    components = new[]
                {
                    new
                    {
                        id = originalComponent.Id,
                        appId = createdApp.Id,
                        name = updatedComponentName,
                        description = "Updated component",
                        key = originalComponent.Key,
                        resourceKey = originalComponent.ResourceKey,
                        content = "<div>Updated</div>",
                        script = string.Empty,
                    },
                },
});

        Component actualComponent = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Components.Single(predicate: component => component.Id == originalComponent.Id);

        // Then

        actualComponent.Name.Should()
            .Be(expected: updatedComponentName);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Delete_RemovesComponentRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "ComponentApp"),
    domain = $"{Unique(prefix: "component")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    components = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Component"),
                        description = "Acceptance component",
                        key = Unique(prefix: "component-key"),
                        resourceKey = "Default",
                        content = "<div>Component</div>",
                        script = string.Empty,
                    },
                },
});

        // When
        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(appId: createdApp.Id);

        // Then

        actualCounts.AppExists.Should()
            .BeFalse();

        actualCounts.ComponentCount.Should()
            .Be(expected: 0);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Post_CreatesScriptRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string scriptName = Unique(prefix: "Script");

        // When

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "ScriptApp"),
    domain = $"{Unique(prefix: "script")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    scripts = new[]
                {
                    new
                    {
                        name = scriptName,
                        description = "Acceptance script",
                        key = Unique(prefix: "script-key"),
                        content = "console.log('script');",
                    },
                },
});

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);

        // Then

        actualChildren.Scripts.Should()
            .ContainSingle(predicate: script => script.Name == scriptName && script.AppId == createdApp.Id);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Put_UpdatesScriptRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "ScriptApp"),
    domain = $"{Unique(prefix: "script")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    scripts = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Script"),
                        description = "Original script",
                        key = Unique(prefix: "script-key"),
                        content = "console.log('original');",
                    },
                },
});

        Script originalScript = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Scripts.Single();
        string updatedScriptName = Unique(prefix: "UpdatedScript");

        // When

        await UpdateAppAsync(
host: createdApp.Domain,
id: createdApp.Id,
payload: new
{
    id = createdApp.Id,
    name = createdApp.Name,
    domain = createdApp.Domain,
    defaultTheme = createdApp.DefaultTheme,
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{\"updated\":true}",
    scripts = new[]
                {
                    new
                    {
                        id = originalScript.Id,
                        appId = createdApp.Id,
                        name = updatedScriptName,
                        description = "Updated script",
                        key = originalScript.Key,
                        content = "console.log('updated');",
                    },
                },
});

        Script actualScript = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Scripts.Single(predicate: script => script.Id == originalScript.Id);

        // Then

        actualScript.Name.Should()
            .Be(expected: updatedScriptName);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Delete_RemovesScriptRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "ScriptApp"),
    domain = $"{Unique(prefix: "script")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    scripts = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Script"),
                        description = "Acceptance script",
                        key = Unique(prefix: "script-key"),
                        content = "console.log('script');",
                    },
                },
});

        // When
        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(appId: createdApp.Id);

        // Then

        actualCounts.AppExists.Should()
            .BeFalse();

        actualCounts.ScriptCount.Should()
            .Be(expected: 0);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Post_CreatesTemplateRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string templateName = Unique(prefix: "Template");

        // When

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "TemplateApp"),
    domain = $"{Unique(prefix: "template")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    templates = new[]
                {
                    new
                    {
                        name = templateName,
                        description = "Acceptance template",
                        resourceKey = "Default",
                        rawString = "<div>Template</div>",
                    },
                },
});

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);

        // Then

        actualChildren.Templates.Should()
            .ContainSingle(predicate: template => template.Name == templateName && template.AppId == createdApp.Id);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Put_UpdatesTemplateRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "TemplateApp"),
    domain = $"{Unique(prefix: "template")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    templates = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Template"),
                        description = "Original template",
                        resourceKey = "Default",
                        rawString = "<div>Original</div>",
                    },
                },
});

        Template originalTemplate = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Templates.Single();
        string updatedTemplateName = Unique(prefix: "UpdatedTemplate");

        // When

        await UpdateAppAsync(
host: createdApp.Domain,
id: createdApp.Id,
payload: new
{
    id = createdApp.Id,
    name = createdApp.Name,
    domain = createdApp.Domain,
    defaultTheme = createdApp.DefaultTheme,
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{\"updated\":true}",
    templates = new[]
                {
                    new
                    {
                        id = originalTemplate.Id,
                        appId = createdApp.Id,
                        name = updatedTemplateName,
                        description = "Updated template",
                        resourceKey = originalTemplate.ResourceKey,
                        rawString = "<div>Updated</div>",
                    },
                },
});

        Template actualTemplate = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Templates.Single(predicate: template => template.Id == originalTemplate.Id);

        // Then

        actualTemplate.Name.Should()
            .Be(expected: updatedTemplateName);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Delete_RemovesTemplateRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "TemplateApp"),
    domain = $"{Unique(prefix: "template")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    templates = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Template"),
                        description = "Acceptance template",
                        resourceKey = "Default",
                        rawString = "<div>Template</div>",
                    },
                },
});

        // When
        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(appId: createdApp.Id);

        // Then

        actualCounts.AppExists.Should()
            .BeFalse();

        actualCounts.TemplateCount.Should()
            .Be(expected: 0);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Post_CreatesRoleRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string roleName = Unique(prefix: "Role");

        // When

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "RoleApp"),
    domain = $"{Unique(prefix: "role")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    roles = new[]
                {
                    new
                    {
                        id = Guid.NewGuid(),
                        name = roleName,
                        description = "Acceptance role",
                        privs = "page_read",
                    },
                },
});

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);

        // Then

        actualChildren.Roles.Should()
            .Contain(predicate: role => role.Name == roleName && role.AppId == createdApp.Id);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Put_UpdatesRoleRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        Guid roleId = Guid.NewGuid();

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "RoleApp"),
    domain = $"{Unique(prefix: "role")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    roles = new[]
                {
                    new
                    {
                        id = roleId,
                        name = Unique(prefix: "Role"),
                        description = "Original role",
                        privs = "page_read",
                    },
                },
});

        Role originalRole = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Roles.Single(predicate: role => role.Id == roleId);
        string updatedRoleName = Unique(prefix: "UpdatedRole");

        // When

        await UpdateAppAsync(
host: createdApp.Domain,
id: createdApp.Id,
payload: new
{
    id = createdApp.Id,
    name = createdApp.Name,
    domain = createdApp.Domain,
    defaultTheme = createdApp.DefaultTheme,
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{\"updated\":true}",
    roles = new[]
                {
                    new
                    {
                        id = originalRole.Id,
                        appId = createdApp.Id,
                        name = updatedRoleName,
                        description = "Updated role",
                        privs = "page_read,page_update",
                    },
                },
});

        Role actualRole = (await GetAppCmsChildrenAsync(appId: createdApp.Id)).Roles.Single(predicate: role => role.Id == originalRole.Id);

        // Then

        actualRole.Name.Should()
            .Be(expected: updatedRoleName);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Delete_RemovesRoleRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "RoleApp"),
    domain = $"{Unique(prefix: "role")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    roles = new[]
                {
                    new
                    {
                        id = Guid.NewGuid(),
                        name = Unique(prefix: "Role"),
                        description = "Acceptance role",
                        privs = "page_read",
                    },
                },
});

        // When
        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(appId: createdApp.Id);

        // Then

        actualCounts.AppExists.Should()
            .BeFalse();

        actualCounts.RoleCount.Should()
            .Be(expected: 0);

        actualCounts.UserRoleCount.Should()
            .Be(expected: 0);

        await Teardown(seededApp: seededApp);
    }
}