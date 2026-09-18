// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task ImportPackage_WhenCommonCachePackageProvided_PersistsAndCachesCommonObjectAsync()
    {
        // Given
        string name = Unique(prefix: "PackageCommonComponent");
        string key = Unique(prefix: "PackageCommonKey");

        Package package = new()
        {
            Category = key,
            Items =
            [
                new PackageItem
                {
                    Type = "ContentManagement/Component",
                    Data = $$"""
                             {
                               "Name": "{{name}}",
                               "ResourceKey": "{{key}}",
                               "Content": "<div>package-common-marker</div>",
                               "Script": ""
                             }
                             """
                }
            ]
        };

        int commonObjectId = 0;

        try
        {
            // When
            using HttpResponseMessage response = await Client.PostAsJsonAsync(
                requestUri: "/Api/ContentManagement/Package/Import",
                value: package);

            string responseContent = await response.Content.ReadAsStringAsync();

            CommonObject storedObject;

            await using (AsyncServiceScope scope =
                fixture.Factory.Services.CreateAsyncScope())
            {
                await using CoreDataContext core = scope.ServiceProvider
                    .GetRequiredService<ICoreContextFactory>()
                    .CreateCoreContext();

                storedObject = await core.Set<CommonObject>()
                    .IgnoreQueryFilters()
                    .SingleAsync(predicate: item =>
                        item.Name == name
                        && item.Key == key
                        && item.Type == "ContentManagement/Component");
            }

            using IServiceScope cacheScope = fixture.Factory.Services.CreateScope();

            CommonObject cachedObject = cacheScope.ServiceProvider
                .GetRequiredService<ICommonObjectCache>()
                .GetLatestSet()
                .Single(predicate: item => item.Id == storedObject.Id);

            commonObjectId = storedObject.Id;

            // Then
            response.StatusCode.Should()
                .Be(expected: HttpStatusCode.OK, because: responseContent);

            storedObject.Json.Should()
                .Contain(expected: "package-common-marker");

            cachedObject.Json.Should()
                .Be(expected: storedObject.Json);
        }
        finally
        {
            if (commonObjectId > 0)
            {
                await using AsyncServiceScope scope =
                    fixture.Factory.Services.CreateAsyncScope();

                await using CoreDataContext core = scope.ServiceProvider
                    .GetRequiredService<ICoreContextFactory>()
                    .CreateCoreContext();

                CommonObject commonObject = await core.Set<CommonObject>()
                    .IgnoreQueryFilters()
                    .SingleAsync(predicate: item => item.Id == commonObjectId);

                core.Remove(entity: commonObject);
                await core.SaveChangesAsync();

                using IServiceScope cacheScope = fixture.Factory.Services.CreateScope();

                cacheScope.ServiceProvider
                    .GetRequiredService<ICommonObjectCache>()
                    .Refresh();
            }
        }
    }

    [Fact]
    public async Task ImportPackage_WhenRepresentativeGraphProvided_PersistsEntireGraphAsync()
    {
        // Given
        string[] privileges =
        [
            "app_admin",
            "package_create",
            "component_create",
            "component_update",
            "layout_create",
            "layout_update",
            "page_create",
            "page_update",
            "resource_create",
            "resource_update",
            "script_create",
            "script_update",
            "template_create",
            "template_update"
        ];

        SeededApp app = await SeedDatabase(privileges: privileges);

        try
        {
            Package package = CreateRepresentativeContentManagementPackage();

            // When
            int statusCode = await ImportPackageAsync(
                appId: app.AppId,
                package: package);

            ImportGraph graph = await GetImportGraphAsync(appId: app.AppId);

            // Then
            statusCode.Should()
                .Be(expected: (int)HttpStatusCode.OK);

            graph.Components.Should()
                .ContainSingle();

            graph.Layouts.Should()
                .ContainSingle();

            graph.Pages.Should()
                .ContainSingle();

            graph.PageInfo.Should()
                .ContainSingle();

            graph.Contents.Should()
                .HaveCount(expected: 2);

            graph.Resources.Should()
                .ContainSingle();

            graph.Scripts.Should()
                .ContainSingle();

            graph.Templates.Should()
                .ContainSingle();
        }
        finally
        {
            await DeleteAppAsync(id: app.AppId);
        }
    }

    [Fact]
    public async Task ImportPackage_CreatesResourcesForApp()
    {
        // Given
        // When
        SeededApp app = await SeedDatabase(privileges: ["app_admin", "package_create", "resource_create", "resource_update"]);

        try
        {
            Package package = AcceptanceSeedData
                .LoadExportPackages()
                .First(predicate: found => string.Equals(a: found.Name, b: "Resources", comparisonType: StringComparison.OrdinalIgnoreCase));

            AppCmsChildren beforeImport = await GetAppCmsChildrenAsync(appId: app.AppId);
            int statusCode = await ImportPackageAsync(appId: app.AppId, package: package);
            AppCmsChildren afterImport = await GetAppCmsChildrenAsync(appId: app.AppId);

            // Then
            statusCode.Should()
                .Be(expected: (int)HttpStatusCode.OK);

            beforeImport.Resources.Should()
                .BeEmpty();

            afterImport.Resources.Should()
                .NotBeEmpty();
        }
        finally
        {
            await DeleteAppAsync(id: app.AppId);
        }
    }

    private async Task<int> ImportPackageAsync(int appId, Package package)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
requestUri: $"/Api/ContentManagement/Package/Import?appId={appId}",
value: package);

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<ImportGraph> GetImportGraphAsync(int appId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        int[] pageIds = await core.Set<Page>()
            .IgnoreQueryFilters()
            .Where(predicate: page => page.AppId == appId)
            .Select(selector: page => page.Id)
            .ToArrayAsync();

        return new ImportGraph(
            Components: core.Set<Component>()
                .IgnoreQueryFilters()
                .Where(predicate: item => item.AppId == appId)
                .AsEnumerable()
                .Select(selector: item => $"{item.Name}|{item.Description}|{item.ResourceKey}|{item.Content}|{item.Script}|{item.Key}")
                .OrderBy(keySelector: item => item)
                .ToArray(),
            Layouts: core.Set<Layout>()
                .IgnoreQueryFilters()
                .Where(predicate: item => item.AppId == appId)
                .AsEnumerable()
                .Select(selector: item => $"{item.Name}|{item.Description}|{item.HeaderHtml}|{item.Html}|{item.Script}")
                .OrderBy(keySelector: item => item)
                .ToArray(),
            Pages: core.Set<Page>()
                .IgnoreQueryFilters()
                .Where(predicate: item => item.AppId == appId)
                .AsEnumerable()
                .Select(selector: item => $"{item.Name}|{item.Order}|{item.ShowOnMenus}|{item.Path}|{item.ResourceKey}|{item.Layout}")
                .OrderBy(keySelector: item => item)
                .ToArray(),
            PageInfo: core.Set<PageInfo>()
                .IgnoreQueryFilters()
                .Where(predicate: item => pageIds.Contains(value: item.PageId))
                .AsEnumerable()
                .Select(selector: item => $"{item.Page.Name}|{item.CultureId}|{item.Title}|{item.Description}|{item.Keywords}")
                .OrderBy(keySelector: item => item)
                .ToArray(),
            Contents: core.Set<Content>()
                .IgnoreQueryFilters()
                .Where(predicate: item => pageIds.Contains(value: item.PageId))
                .AsEnumerable()
                .Select(selector: item => $"{item.Page.Name}|{item.CultureId}|{item.Name}|{item.Html}")
                .OrderBy(keySelector: item => item)
                .ToArray(),
            Resources: core.Set<Resource>()
                .IgnoreQueryFilters()
                .Where(predicate: item => item.AppId == appId)
                .AsEnumerable()
                .Select(selector: item => $"{item.Name}|{item.Description}|{item.Key}|{item.Culture}|{item.DisplayName}|{item.ShortDisplayName}")
                .OrderBy(keySelector: item => item)
                .ToArray(),
            Scripts: core.Set<Script>()
                .IgnoreQueryFilters()
                .Where(predicate: item => item.AppId == appId)
                .AsEnumerable()
                .Select(selector: item => $"{item.Name}|{item.Description}|{item.Key}|{item.Content}")
                .OrderBy(keySelector: item => item)
                .ToArray(),
            Templates: core.Set<Template>()
                .IgnoreQueryFilters()
                .Where(predicate: item => item.AppId == appId)
                .AsEnumerable()
                .Select(selector: item => $"{item.Name}|{item.Description}|{item.ResourceKey}|{item.RawString}")
                .OrderBy(keySelector: item => item)
                .ToArray());
    }

    private static Package CreateRepresentativeContentManagementPackage()
    {
        string[] packageNames =
        [
            "Components",
            "Layouts",
            "Pages",
            "Resources",
            "Templates"
        ];

        Package[] sourcePackages = AcceptanceSeedData.LoadExportPackages();

        return new Package
        {
            Name = "Content management import equivalence",
            Items =
            [
                .. packageNames.Select(selector: packageName =>
                {
                    PackageItem sourceItem = sourcePackages
                        .First(predicate: source => source.Name == packageName)
                        .Items.Single();

                    using JsonDocument document = JsonDocument.Parse(
                        json: sourceItem.Data);

                    string data = document.RootElement.ValueKind
                        == JsonValueKind.Array
                            ? document.RootElement
                                .EnumerateArray()
                                .First()
                                .GetRawText()
                            : document.RootElement.GetRawText();

                    return new PackageItem
                    {
                        Type = sourceItem.Type,
                        Data = data
                    };
                }),
                new PackageItem
                {
                    Type = "ContentManagement/Script",
                    Data = """
                           {
                             "Name": "EquivalenceScript",
                             "Description": "Import equivalence script",
                             "Key": "Acceptance",
                             "Content": "window.importEquivalent = true;"
                           }
                           """
                }
            ]
        };
    }

    private sealed record ImportGraph(
        string[] Components,
        string[] Layouts,
        string[] Pages,
        string[] PageInfo,
        string[] Contents,
        string[] Resources,
        string[] Scripts,
        string[] Templates);
}