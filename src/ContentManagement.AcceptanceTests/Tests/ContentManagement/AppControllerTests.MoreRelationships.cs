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
        SeededApp seededApp = await SeedDatabase("app_create");
        string cultureId = await GetNonDefaultCultureIdAsync();

        // When
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("CultureApp"),
                domain = $"{Unique("culture")}.local",
                defaultTheme = "Default",
                defaultCultureId = cultureId,
                tenantId = Unique("tenant"),
                configJson = "{}",
                cultures = new[] { new { cultureId } },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);

        // Then
        actualChildren.Cultures.Select(culture => culture.CultureId).Should().Contain([string.Empty, cultureId]);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Put_UpdatesAppCultureRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        IReadOnlyList<string> cultureIds = await GetNonDefaultCultureIdsAsync(2);
        string originalCultureId = cultureIds[0];
        string updatedCultureId = cultureIds[1];

        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("CultureApp"),
                domain = $"{Unique("culture")}.local",
                defaultTheme = "Default",
                defaultCultureId = originalCultureId,
                tenantId = Unique("tenant"),
                configJson = "{}",
                cultures = new[] { new { cultureId = originalCultureId } },
            });

        // When
        await UpdateAppAsync(
            createdApp.Domain,
            createdApp.Id,
            new
            {
                id = createdApp.Id,
                name = createdApp.Name,
                domain = createdApp.Domain,
                defaultTheme = createdApp.DefaultTheme,
                defaultCultureId = updatedCultureId,
                tenantId = Unique("tenant"),
                configJson = "{\"updated\":true}",
                cultures = new[] { new { appId = createdApp.Id, cultureId = updatedCultureId } },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);

        // Then
        actualChildren.Cultures.Select(culture => culture.CultureId).Should().Contain([string.Empty, updatedCultureId]);
        actualChildren.Cultures.Select(culture => culture.CultureId).Should().NotContain(originalCultureId);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Delete_RemovesAppCultureRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        string cultureId = await GetNonDefaultCultureIdAsync();
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("CultureApp"),
                domain = $"{Unique("culture")}.local",
                defaultTheme = "Default",
                defaultCultureId = cultureId,
                tenantId = Unique("tenant"),
                configJson = "{}",
                cultures = new[] { new { cultureId } },
            });

        // When
        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(createdApp.Id);

        // Then
        actualCounts.AppExists.Should().BeFalse();
        actualCounts.AppCultureCount.Should().Be(0);

        await Teardown(seededApp);
    }

    [Fact]
    public async Task Post_CreatesComponentRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        string componentName = Unique("Component");

        // When
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("ComponentApp"),
                domain = $"{Unique("component")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                components = new[]
                {
                    new
                    {
                        name = componentName,
                        description = "Acceptance component",
                        key = Unique("component-key"),
                        resourceKey = "Default",
                        content = "<div>Component</div>",
                        script = string.Empty,
                    },
                },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);

        // Then
        actualChildren.Components.Should().ContainSingle(component => component.Name == componentName && component.AppId == createdApp.Id);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Put_UpdatesComponentRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("ComponentApp"),
                domain = $"{Unique("component")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                components = new[]
                {
                    new
                    {
                        name = Unique("Component"),
                        description = "Original component",
                        key = Unique("component-key"),
                        resourceKey = "Default",
                        content = "<div>Original</div>",
                        script = string.Empty,
                    },
                },
            });

        Component originalComponent = (await GetAppCmsChildrenAsync(createdApp.Id)).Components.Single();
        string updatedComponentName = Unique("UpdatedComponent");

        // When
        await UpdateAppAsync(
            createdApp.Domain,
            createdApp.Id,
            new
            {
                id = createdApp.Id,
                name = createdApp.Name,
                domain = createdApp.Domain,
                defaultTheme = createdApp.DefaultTheme,
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
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

        Component actualComponent = (await GetAppCmsChildrenAsync(createdApp.Id)).Components.Single(component => component.Id == originalComponent.Id);

        // Then
        actualComponent.Name.Should().Be(updatedComponentName);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Delete_RemovesComponentRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("ComponentApp"),
                domain = $"{Unique("component")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                components = new[]
                {
                    new
                    {
                        name = Unique("Component"),
                        description = "Acceptance component",
                        key = Unique("component-key"),
                        resourceKey = "Default",
                        content = "<div>Component</div>",
                        script = string.Empty,
                    },
                },
            });

        // When
        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(createdApp.Id);

        // Then
        actualCounts.AppExists.Should().BeFalse();
        actualCounts.ComponentCount.Should().Be(0);

        await Teardown(seededApp);
    }

    [Fact]
    public async Task Post_CreatesScriptRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        string scriptName = Unique("Script");

        // When
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("ScriptApp"),
                domain = $"{Unique("script")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                scripts = new[]
                {
                    new
                    {
                        name = scriptName,
                        description = "Acceptance script",
                        key = Unique("script-key"),
                        content = "console.log('script');",
                    },
                },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);

        // Then
        actualChildren.Scripts.Should().ContainSingle(script => script.Name == scriptName && script.AppId == createdApp.Id);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Put_UpdatesScriptRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("ScriptApp"),
                domain = $"{Unique("script")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                scripts = new[]
                {
                    new
                    {
                        name = Unique("Script"),
                        description = "Original script",
                        key = Unique("script-key"),
                        content = "console.log('original');",
                    },
                },
            });

        Script originalScript = (await GetAppCmsChildrenAsync(createdApp.Id)).Scripts.Single();
        string updatedScriptName = Unique("UpdatedScript");

        // When
        await UpdateAppAsync(
            createdApp.Domain,
            createdApp.Id,
            new
            {
                id = createdApp.Id,
                name = createdApp.Name,
                domain = createdApp.Domain,
                defaultTheme = createdApp.DefaultTheme,
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
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

        Script actualScript = (await GetAppCmsChildrenAsync(createdApp.Id)).Scripts.Single(script => script.Id == originalScript.Id);

        // Then
        actualScript.Name.Should().Be(updatedScriptName);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Delete_RemovesScriptRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("ScriptApp"),
                domain = $"{Unique("script")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                scripts = new[]
                {
                    new
                    {
                        name = Unique("Script"),
                        description = "Acceptance script",
                        key = Unique("script-key"),
                        content = "console.log('script');",
                    },
                },
            });

        // When
        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(createdApp.Id);

        // Then
        actualCounts.AppExists.Should().BeFalse();
        actualCounts.ScriptCount.Should().Be(0);

        await Teardown(seededApp);
    }

    [Fact]
    public async Task Post_CreatesTemplateRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        string templateName = Unique("Template");

        // When
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("TemplateApp"),
                domain = $"{Unique("template")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
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

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);

        // Then
        actualChildren.Templates.Should().ContainSingle(template => template.Name == templateName && template.AppId == createdApp.Id);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Put_UpdatesTemplateRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("TemplateApp"),
                domain = $"{Unique("template")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                templates = new[]
                {
                    new
                    {
                        name = Unique("Template"),
                        description = "Original template",
                        resourceKey = "Default",
                        rawString = "<div>Original</div>",
                    },
                },
            });

        Template originalTemplate = (await GetAppCmsChildrenAsync(createdApp.Id)).Templates.Single();
        string updatedTemplateName = Unique("UpdatedTemplate");

        // When
        await UpdateAppAsync(
            createdApp.Domain,
            createdApp.Id,
            new
            {
                id = createdApp.Id,
                name = createdApp.Name,
                domain = createdApp.Domain,
                defaultTheme = createdApp.DefaultTheme,
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
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

        Template actualTemplate = (await GetAppCmsChildrenAsync(createdApp.Id)).Templates.Single(template => template.Id == originalTemplate.Id);

        // Then
        actualTemplate.Name.Should().Be(updatedTemplateName);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Delete_RemovesTemplateRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("TemplateApp"),
                domain = $"{Unique("template")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                templates = new[]
                {
                    new
                    {
                        name = Unique("Template"),
                        description = "Acceptance template",
                        resourceKey = "Default",
                        rawString = "<div>Template</div>",
                    },
                },
            });

        // When
        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(createdApp.Id);

        // Then
        actualCounts.AppExists.Should().BeFalse();
        actualCounts.TemplateCount.Should().Be(0);

        await Teardown(seededApp);
    }

    [Fact]
    public async Task Post_CreatesRoleRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        string roleName = Unique("Role");

        // When
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("RoleApp"),
                domain = $"{Unique("role")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
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

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);

        // Then
        actualChildren.Roles.Should().Contain(role => role.Name == roleName && role.AppId == createdApp.Id);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Put_UpdatesRoleRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        Guid roleId = Guid.NewGuid();
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("RoleApp"),
                domain = $"{Unique("role")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                roles = new[]
                {
                    new
                    {
                        id = roleId,
                        name = Unique("Role"),
                        description = "Original role",
                        privs = "page_read",
                    },
                },
            });

        Role originalRole = (await GetAppCmsChildrenAsync(createdApp.Id)).Roles.Single(role => role.Id == roleId);
        string updatedRoleName = Unique("UpdatedRole");

        // When
        await UpdateAppAsync(
            createdApp.Domain,
            createdApp.Id,
            new
            {
                id = createdApp.Id,
                name = createdApp.Name,
                domain = createdApp.Domain,
                defaultTheme = createdApp.DefaultTheme,
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
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

        Role actualRole = (await GetAppCmsChildrenAsync(createdApp.Id)).Roles.Single(role => role.Id == originalRole.Id);

        // Then
        actualRole.Name.Should().Be(updatedRoleName);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Delete_RemovesRoleRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("RoleApp"),
                domain = $"{Unique("role")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                roles = new[]
                {
                    new
                    {
                        id = Guid.NewGuid(),
                        name = Unique("Role"),
                        description = "Acceptance role",
                        privs = "page_read",
                    },
                },
            });

        // When
        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(createdApp.Id);

        // Then
        actualCounts.AppExists.Should().BeFalse();
        actualCounts.RoleCount.Should().Be(0);
        actualCounts.UserRoleCount.Should().Be(0);

        await Teardown(seededApp);
    }
}





