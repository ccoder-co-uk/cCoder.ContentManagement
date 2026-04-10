using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Post_CreatesLayoutRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        string appName = Unique("RelatedApp");
        string appDomain = $"{Unique("related")}.local";
        string layoutName = Unique("Layout");

        // When
        App actualApp = await CreateAppAsync(
            new
            {
                name = appName,
                domain = appDomain,
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                layouts = new[]
                {
                    new
                    {
                        name = layoutName,
                        description = "Acceptance layout",
                        html = "<main>@RenderBody()</main>",
                        headerHtml = "<title>Acceptance</title>",
                        script = string.Empty,
                    },
                },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(actualApp.Id);

        // Then
        actualChildren.Layouts.Should().ContainSingle(layout => layout.Name == layoutName && layout.AppId == actualApp.Id);
        actualChildren.Resources.Should().BeEmpty();

        await DeleteAppAsync(actualApp.Domain, actualApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Post_CreatesResourceRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        string appName = Unique("RelatedApp");
        string appDomain = $"{Unique("related")}.local";
        string resourceName = Unique("Resource");
        string resourceKey = Unique("key");

        // When
        App actualApp = await CreateAppAsync(
            new
            {
                name = appName,
                domain = appDomain,
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                resources = new[]
                {
                    new
                    {
                        name = resourceName,
                        description = "Acceptance resource",
                        key = resourceKey,
                        culture = string.Empty,
                        displayName = "Acceptance Resource",
                        shortDisplayName = "Acceptance",
                    },
                },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(actualApp.Id);

        // Then
        actualChildren.Layouts.Should().BeEmpty();
        actualChildren.Resources.Should().ContainSingle(resource =>
            resource.Name == resourceName
            && resource.Key == resourceKey
            && resource.AppId == actualApp.Id);

        await DeleteAppAsync(actualApp.Domain, actualApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Put_UpdatesLayoutRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("RelatedApp"),
                domain = $"{Unique("related")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                layouts = new[]
                {
                    new
                    {
                        name = Unique("Layout"),
                        description = "Original layout",
                        html = "<main>Original</main>",
                        headerHtml = "<title>Original</title>",
                        script = string.Empty,
                    },
                },
            });

        AppCmsChildren originalChildren = await GetAppCmsChildrenAsync(createdApp.Id);
        Layout originalLayout = originalChildren.Layouts.Single();
        string updatedLayoutName = Unique("UpdatedLayout");

        // When
        await UpdateAppAsync(
            createdApp.Domain,
            createdApp.Id,
            new
            {
                id = createdApp.Id,
                name = createdApp.Name,
                domain = createdApp.Domain,
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{\"updated\":true}",
                layouts = new[]
                {
                    new
                    {
                        id = originalLayout.Id,
                        appId = createdApp.Id,
                        name = updatedLayoutName,
                        description = "Updated layout",
                        html = "<main>Updated</main>",
                        headerHtml = "<title>Updated</title>",
                        script = string.Empty,
                    },
                },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);
        Layout actualLayout = actualChildren.Layouts.Single(layout => layout.Id == originalLayout.Id);

        // Then
        actualLayout.Name.Should().Be(updatedLayoutName);
        actualChildren.Resources.Should().BeEmpty();

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Put_UpdatesResourceRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("RelatedApp"),
                domain = $"{Unique("related")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                resources = new[]
                {
                    new
                    {
                        name = Unique("Resource"),
                        description = "Original resource",
                        key = Unique("key"),
                        culture = string.Empty,
                        displayName = "Original Resource",
                        shortDisplayName = "Original",
                    },
                },
            });

        AppCmsChildren originalChildren = await GetAppCmsChildrenAsync(createdApp.Id);
        Resource originalResource = originalChildren.Resources.Single();
        string updatedResourceName = Unique("UpdatedResource");
        string updatedDisplayName = "Updated Resource";

        // When
        await UpdateAppAsync(
            createdApp.Domain,
            createdApp.Id,
            new
            {
                id = createdApp.Id,
                name = createdApp.Name,
                domain = createdApp.Domain,
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{\"updated\":true}",
                resources = new[]
                {
                    new
                    {
                        id = originalResource.Id,
                        appId = createdApp.Id,
                        name = updatedResourceName,
                        description = "Updated resource",
                        key = originalResource.Key,
                        culture = string.Empty,
                        displayName = updatedDisplayName,
                        shortDisplayName = "Updated",
                    },
                },
            });

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(createdApp.Id);
        Resource actualResource = actualChildren.Resources.Single(resource => resource.Id == originalResource.Id);

        // Then
        actualChildren.Layouts.Should().BeEmpty();
        actualResource.Name.Should().Be(updatedResourceName);
        actualResource.DisplayName.Should().Be(updatedDisplayName);

        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        await Teardown(seededApp);
    }

    [Fact]
    public async Task Delete_RemovesLayoutRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("RelatedApp"),
                domain = $"{Unique("related")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                layouts = new[]
                {
                    new
                    {
                        name = Unique("Layout"),
                        description = "Acceptance layout",
                        html = "<main>Acceptance</main>",
                        headerHtml = "<title>Acceptance</title>",
                        script = string.Empty,
                    },
                },
            });

        // When
        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(createdApp.Id);

        // Then
        actualCounts.AppExists.Should().BeFalse();
        actualCounts.LayoutCount.Should().Be(0);

        await Teardown(seededApp);
    }

    [Fact]
    public async Task Delete_RemovesResourceRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_create");
        App createdApp = await CreateAppAsync(
            new
            {
                name = Unique("RelatedApp"),
                domain = $"{Unique("related")}.local",
                defaultTheme = "Default",
                defaultCultureId = string.Empty,
                tenantId = Unique("tenant"),
                configJson = "{}",
                resources = new[]
                {
                    new
                    {
                        name = Unique("Resource"),
                        description = "Acceptance resource",
                        key = Unique("key"),
                        culture = string.Empty,
                        displayName = "Acceptance Resource",
                        shortDisplayName = "Acceptance",
                    },
                },
            });

        // When
        await DeleteAppAsync(createdApp.Domain, createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(createdApp.Id);

        // Then
        actualCounts.AppExists.Should().BeFalse();
        actualCounts.ResourceCount.Should().Be(0);

        await Teardown(seededApp);
    }
}





