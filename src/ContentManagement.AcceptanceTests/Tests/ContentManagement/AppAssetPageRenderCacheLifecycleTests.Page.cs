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
    public async Task PageUpdate_InvalidatesPageThenRebuildsChangedContentAsync()
    {
        // Given
        string suffix = Guid.NewGuid().ToString(format: "N");
        string pageName = $"LifecyclePage{suffix}";
        string pagePath = $"lifecycle-page-{suffix}";
        const string originalMarker = "original-page-content-marker";
        const string updatedMarker = "updated-page-content-marker";

        int pageId = await SeedContentPageAsync(
            pageName: pageName,
            pagePath: pagePath,
            marker: originalMarker);

        PageRenderResponse firstResponse = await RenderAsync(path: pagePath);
        PageRenderCache firstCache = await GetCacheAsync(pageId: pageId);

        // When
        using HttpResponseMessage updateResponse =
            await fixture.Client.PutAsJsonAsync(
                requestUri: $"/Api/ContentManagement/Page({pageId})",
                value: new
                {
                    id = pageId,
                    appId = 1,
                    name = pageName,
                    path = pagePath,
                    order = 1,
                    showOnMenus = false,
                    resourceKey = "Default",
                    layout = firstResponse.Page.Layout,
                    pageInfo = new[]
                    {
                        new
                        {
                            cultureId = string.Empty,
                            title = pageName,
                            description = "Lifecycle page updated",
                            keywords = "lifecycle,page"
                        }
                    },
                    contents = new[]
                    {
                        new
                        {
                            cultureId = string.Empty,
                            name = "body",
                            html = updatedMarker
                        }
                    }
                });

        string updateContent = await updateResponse.Content
            .ReadAsStringAsync();

        Page storedPage = await GetPageAsync(pageId: pageId);
        PageRenderCache[] cachesAfterUpdate = await GetCachesAsync(
            pageId: pageId);

        PageRenderResponse rebuiltResponse = await RenderAsync(
            path: storedPage.Path);
        PageRenderCache rebuiltCache = await GetCacheAsync(pageId: pageId);

        PageRenderResponse cachedResponse = await RenderAsync(
            path: storedPage.Path);
        PageRenderCache cachedAgain = await GetCacheAsync(pageId: pageId);

        // Then
        using AssertionScope assertionScope = new();

        updateResponse.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: updateContent);

        firstResponse.Page.BodyHtml.Should()
            .Contain(expected: originalMarker);

        firstCache.Body.Should()
            .Contain(expected: originalMarker);

        storedPage.Contents.Should()
            .ContainSingle(predicate: content =>
                content.Name == "body" && content.Html == updatedMarker);

        cachesAfterUpdate.Should()
            .BeEmpty(
                because: "a page update must invalidate rendered variants for that page");

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

    private async Task<int> SeedContentPageAsync(
        string pageName,
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
            body: "<main>[content[body]]</main>",
            timestamp: now);

        Page page = CreatePage(
            name: pageName,
            path: pagePath,
            layoutName: layout.Name,
            timestamp: now);

        page.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = pageName,
                Description = "Lifecycle page",
                Keywords = "lifecycle,page"
            }
        ];

        page.Contents =
        [
            new Content
            {
                CultureId = string.Empty,
                Name = "body",
                Html = marker
            }
        ];

        core.AddRange(layout, page);
        await core.SaveChangesAsync();
        return page.Id;
    }

    private async Task<Page> GetPageAsync(int pageId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await core.Pages
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: page => page.Contents)
            .SingleAsync(predicate: item => item.Id == pageId);
    }
}
