// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppAssetPageRenderCacheLifecycleTests
{
    [Fact]
    public async Task ResourceUpdate_InvalidatesAppThenRebuildsChangedPageAsync()
    {
        // Given
        string suffix = Guid.NewGuid().ToString(format: "N");
        string resourceName = $"LifecycleResource{suffix}";
        string pagePath = $"lifecycle-resource-{suffix}";
        const string originalMarker = "original-app-resource-marker";
        const string updatedMarker = "updated-app-resource-marker";

        (int PageId, int ResourceId) seeded = await SeedResourcePageAsync(
            resourceName: resourceName,
            pagePath: pagePath,
            marker: originalMarker);

        PageRenderResponse firstResponse = await RenderAsync(path: pagePath);
        PageRenderCache firstCache = await GetCacheAsync(
            pageId: seeded.PageId);

        // When
        using HttpResponseMessage updateResponse =
            await fixture.Client.PutAsJsonAsync(
                requestUri:
                    $"/Api/ContentManagement/Resource({seeded.ResourceId})",
                value: new
                {
                    id = seeded.ResourceId,
                    appId = 1,
                    name = resourceName,
                    description = "Lifecycle resource updated",
                    key = "Default",
                    culture = string.Empty,
                    displayName = updatedMarker,
                    shortDisplayName = updatedMarker
                });

        string updateContent = await updateResponse.Content
            .ReadAsStringAsync();

        Resource storedResource = await GetResourceAsync(
            resourceId: seeded.ResourceId);

        PageRenderCache[] cachesAfterUpdate = await GetCachesAsync(
            pageId: seeded.PageId);

        PageRenderResponse rebuiltResponse = await RenderAsync(path: pagePath);
        PageRenderCache rebuiltCache = await GetCacheAsync(
            pageId: seeded.PageId);

        PageRenderResponse cachedResponse = await RenderAsync(path: pagePath);
        PageRenderCache cachedAgain = await GetCacheAsync(
            pageId: seeded.PageId);

        // Then
        using AssertionScope assertionScope = new();

        updateResponse.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: updateContent);

        firstResponse.Page.BodyHtml.Should()
            .Contain(expected: originalMarker);

        firstCache.Body.Should()
            .Contain(expected: originalMarker);

        storedResource.DisplayName.Should()
            .Be(expected: updatedMarker);

        cachesAfterUpdate.Should()
            .BeEmpty(
                because: "an app resource update must invalidate rendered variants for its app");

        rebuiltResponse.Page.BodyHtml.Should()
            .Contain(expected: updatedMarker);

        rebuiltCache.Body.Should()
            .Contain(expected: updatedMarker);

        cachedResponse.Page.BodyHtml.Should()
            .Be(expected: rebuiltResponse.Page.BodyHtml);

        cachedAgain.Id.Should()
            .Be(expected: rebuiltCache.Id);

        cachedAgain.RenderedOn.Should()
            .Be(expected: rebuiltCache.RenderedOn);
    }

    private async Task<(int PageId, int ResourceId)> SeedResourcePageAsync(
        string resourceName,
        string pagePath,
        string marker)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        DateTimeOffset now = DateTimeOffset.UtcNow.AddMinutes(minutes: -2);
        Layout layout = CreateLayout(
            name: $"LifecycleLayout{Guid.NewGuid():N}",
            body: $"<main>[resource_displayname[{resourceName}]]</main>",
            timestamp: now);

        Page page = CreatePage(
            name: $"LifecyclePage{Guid.NewGuid():N}",
            path: pagePath,
            layoutName: layout.Name,
            timestamp: now);

        Resource resource = new()
        {
            AppId = 1,
            Name = resourceName,
            Description = "Lifecycle resource",
            Key = "Default",
            Culture = string.Empty,
            DisplayName = marker,
            ShortDisplayName = marker,
            CreatedOn = now,
            CreatedBy = "Guest",
            LastUpdated = now,
            LastUpdatedBy = "Guest"
        };

        core.AddRange(layout, page, resource);
        await core.SaveChangesAsync();

        return (PageId: page.Id, ResourceId: resource.Id);
    }

    private async Task<Resource> GetResourceAsync(int resourceId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await core.Resources
            .IgnoreQueryFilters()
            .SingleAsync(predicate: item => item.Id == resourceId);
    }
}
