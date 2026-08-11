// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppAssetPageRenderCacheLifecycleTests
{
    [Fact]
    public async Task AppPackageImport_InvalidatesAppThenRebuildsChangedPageAsync()
    {
        // Given
        string suffix = Guid.NewGuid().ToString(format: "N");
        string componentName = $"LifecyclePackageComponent{suffix}";
        string pagePath = $"lifecycle-package-{suffix}";
        const string originalMarker = "original-package-component-marker";
        const string importedMarker = "imported-package-component-marker";

        (int PageId, int ComponentId) seeded = await SeedComponentPageAsync(
            componentName: componentName,
            pagePath: pagePath,
            marker: originalMarker);

        PageRenderResponse firstResponse = await RenderAsync(path: pagePath);
        PageRenderCache firstCache = await GetCacheAsync(
            pageId: seeded.PageId);

        Package package = new()
        {
            Name = $"LifecyclePackage{suffix}",
            Items =
            [
                new PackageItem
                {
                    Type = "ContentManagement/Component",
                    Data = JsonConvert.SerializeObject(value: new[]
                    {
                        new Component
                        {
                            Name = componentName,
                            Description = "Lifecycle package component updated",
                            ResourceKey = "Default",
                            Content = importedMarker,
                            Script = string.Empty,
                            Key = "Acceptance"
                        }
                    })
                }
            ]
        };

        // When
        using HttpResponseMessage importResponse =
            await fixture.Client.PostAsJsonAsync(
                requestUri: "/Api/ContentManagement/Package/Import?appId=1",
                value: package);

        string importContent = await importResponse.Content
            .ReadAsStringAsync();

        Component storedComponent = await GetComponentAsync(
            componentId: seeded.ComponentId);

        PageRenderCache[] cachesAfterImport = await GetCachesAsync(
            pageId: seeded.PageId);

        PageRenderResponse rebuiltResponse = await RenderAsync(path: pagePath);
        PageRenderCache rebuiltCache = await GetCacheAsync(
            pageId: seeded.PageId);

        PageRenderResponse cachedResponse = await RenderAsync(path: pagePath);
        PageRenderCache cachedAgain = await GetCacheAsync(
            pageId: seeded.PageId);

        // Then
        using AssertionScope assertionScope = new();

        importResponse.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: importContent);

        firstResponse.Page.BodyHtml.Should()
            .Contain(expected: originalMarker);

        firstCache.Body.Should()
            .Contain(expected: originalMarker);

        storedComponent.Content.Should()
            .Be(
                expected: importedMarker,
                because: "the package import must persist the component update");

        cachesAfterImport.Should()
            .BeEmpty(
                because: "an app package import must invalidate rendered variants for that app");

        rebuiltResponse.Page.BodyHtml.Should()
            .Contain(expected: importedMarker);

        rebuiltCache.Body.Should()
            .Contain(expected: importedMarker);

        cachedResponse.Page.BodyHtml.Should()
            .Be(expected: rebuiltResponse.Page.BodyHtml);

        cachedAgain.Id.Should()
            .Be(expected: rebuiltCache.Id);

        cachedAgain.RenderedOn.Should()
            .Be(expected: rebuiltCache.RenderedOn);
    }
}
