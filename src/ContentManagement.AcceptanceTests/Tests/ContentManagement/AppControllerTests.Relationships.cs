// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string appName = Unique(prefix: "RelatedApp");
        string appDomain = $"{Unique(prefix: "related")}.local";
        string layoutName = Unique(prefix: "Layout");

        // When

        App actualApp = await CreateAppAsync(
payload: new
{
    name = appName,
    domain = appDomain,
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
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

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: actualApp.Id);

        // Then

        actualChildren.Layouts.Should()
            .ContainSingle(predicate: layout => layout.Name == layoutName && layout.AppId == actualApp.Id);

        actualChildren.Resources.Should()
            .BeEmpty();

        await DeleteAppAsync(host: actualApp.Domain, id: actualApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Post_CreatesResourceRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");
        string appName = Unique(prefix: "RelatedApp");
        string appDomain = $"{Unique(prefix: "related")}.local";
        string resourceName = Unique(prefix: "Resource");
        string resourceKey = Unique(prefix: "key");

        // When

        App actualApp = await CreateAppAsync(
payload: new
{
    name = appName,
    domain = appDomain,
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
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

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: actualApp.Id);

        // Then

        actualChildren.Layouts.Should()
            .BeEmpty();

        actualChildren.Resources.Should()
            .ContainSingle(predicate: resource =>
            resource.Name == resourceName
            && resource.Key == resourceKey
            && resource.AppId == actualApp.Id);

        await DeleteAppAsync(host: actualApp.Domain, id: actualApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Put_UpdatesLayoutRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "RelatedApp"),
    domain = $"{Unique(prefix: "related")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    layouts = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Layout"),
                        description = "Original layout",
                        html = "<main>Original</main>",
                        headerHtml = "<title>Original</title>",
                        script = string.Empty,
                    },
                },
});

        AppCmsChildren originalChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);
        Layout originalLayout = originalChildren.Layouts.Single();
        string updatedLayoutName = Unique(prefix: "UpdatedLayout");

        // When

        await UpdateAppAsync(
host: createdApp.Domain,
id: createdApp.Id,
payload: new
{
    id = createdApp.Id,
    name = createdApp.Name,
    domain = createdApp.Domain,
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
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

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);
        Layout actualLayout = actualChildren.Layouts.Single(predicate: layout => layout.Id == originalLayout.Id);

        // Then

        actualLayout.Name.Should()
            .Be(expected: updatedLayoutName);

        actualChildren.Resources.Should()
            .BeEmpty();

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Put_UpdatesResourceRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "RelatedApp"),
    domain = $"{Unique(prefix: "related")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    resources = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Resource"),
                        description = "Original resource",
                        key = Unique(prefix: "key"),
                        culture = string.Empty,
                        displayName = "Original Resource",
                        shortDisplayName = "Original",
                    },
                },
});

        AppCmsChildren originalChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);
        Resource originalResource = originalChildren.Resources.Single();
        string updatedResourceName = Unique(prefix: "UpdatedResource");
        string updatedDisplayName = "Updated Resource";

        // When

        await UpdateAppAsync(
host: createdApp.Domain,
id: createdApp.Id,
payload: new
{
    id = createdApp.Id,
    name = createdApp.Name,
    domain = createdApp.Domain,
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
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

        AppCmsChildren actualChildren = await GetAppCmsChildrenAsync(appId: createdApp.Id);
        Resource actualResource = actualChildren.Resources.Single(predicate: resource => resource.Id == originalResource.Id);

        // Then

        actualChildren.Layouts.Should()
            .BeEmpty();

        actualResource.Name.Should()
            .Be(expected: updatedResourceName);

        actualResource.DisplayName.Should()
            .Be(expected: updatedDisplayName);

        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Delete_RemovesLayoutRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "RelatedApp"),
    domain = $"{Unique(prefix: "related")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    layouts = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Layout"),
                        description = "Acceptance layout",
                        html = "<main>Acceptance</main>",
                        headerHtml = "<title>Acceptance</title>",
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

        actualCounts.LayoutCount.Should()
            .Be(expected: 0);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Delete_RemovesResourceRelationship()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_create");

        App createdApp = await CreateAppAsync(
payload: new
{
    name = Unique(prefix: "RelatedApp"),
    domain = $"{Unique(prefix: "related")}.local",
    defaultTheme = "Default",
    defaultCultureId = string.Empty,
    tenantId = Unique(prefix: "tenant"),
    configJson = "{}",
    resources = new[]
                {
                    new
                    {
                        name = Unique(prefix: "Resource"),
                        description = "Acceptance resource",
                        key = Unique(prefix: "key"),
                        culture = string.Empty,
                        displayName = "Acceptance Resource",
                        shortDisplayName = "Acceptance",
                    },
                },
});

        // When
        await DeleteAppAsync(host: createdApp.Domain, id: createdApp.Id);
        AppChildCounts actualCounts = await GetAppChildCountsAsync(appId: createdApp.Id);

        // Then

        actualCounts.AppExists.Should()
            .BeFalse();

        actualCounts.ResourceCount.Should()
            .Be(expected: 0);

        await Teardown(seededApp: seededApp);
    }
}