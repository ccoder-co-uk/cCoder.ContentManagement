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
    public async Task LayoutUpdate_InvalidatesAppThenRebuildsChangedPageAsync()
    {
        // Given

        string suffix = Guid.NewGuid()
            .ToString(format: "N");

        string layoutName = $"LifecycleLayout{suffix}";
        string pagePath = $"lifecycle-layout-{suffix}";
        const string originalMarker = "original-app-layout-marker";
        const string updatedMarker = "updated-app-layout-marker";

        (int PageId, int LayoutId) seeded = await SeedLayoutPageAsync(
            layoutName: layoutName,
            pagePath: pagePath,
            marker: originalMarker);

        PageRenderResponse firstResponse = await RenderAsync(path: pagePath);

        PageRenderCache firstCache = await GetCacheAsync(
            pageId: seeded.PageId);

        // When
        using HttpResponseMessage updateResponse =
            await fixture.Client.PutAsJsonAsync(
                requestUri:
                    $"/Api/ContentManagement/Layout({seeded.LayoutId})",
                value: new
                {
                    id = seeded.LayoutId,
                    appId = 1,
                    name = layoutName,
                    description = "Lifecycle layout updated",
                    headerHtml = string.Empty,
                    html = $"<main>{updatedMarker}</main>",
                    script = string.Empty
                });

        string updateContent = await updateResponse.Content
            .ReadAsStringAsync();

        Layout storedLayout = await GetLayoutAsync(layoutId: seeded.LayoutId);

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

        storedLayout.Html.Should()
            .Contain(expected: updatedMarker);

        cachesAfterUpdate.Should()
            .BeEmpty(
                because: "an app layout update must invalidate rendered variants for its app");

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

    private async Task<(int PageId, int LayoutId)> SeedLayoutPageAsync(
        string layoutName,
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
            name: layoutName,
            body: $"<main>{marker}</main>",
            timestamp: now);

        Page page = CreatePage(
            name: $"LifecyclePage{Guid.NewGuid():N}",
            path: pagePath,
            layoutName: layout.Name,
            timestamp: now);

        core.AddRange(entities: [layout, page]);
        await core.SaveChangesAsync();

        return (PageId: page.Id, LayoutId: layout.Id);
    }

    private async Task<Layout> GetLayoutAsync(int layoutId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await core.Layouts
            .IgnoreQueryFilters()
            .SingleAsync(predicate: item => item.Id == layoutId);
    }
}