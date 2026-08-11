// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class AppAssetPageRenderCacheLifecycleTests(WebAcceptanceFixture fixture)
{
    [Fact]
    public async Task ComponentUpdate_InvalidatesAppThenRebuildsChangedPageAsync()
    {
        // Given

        string suffix = Guid.NewGuid()
            .ToString(format: "N");

        string componentName = $"LifecycleComponent{suffix}";
        string pagePath = $"lifecycle-component-{suffix}";
        const string originalMarker = "original-app-component-marker";
        const string updatedMarker = "updated-app-component-marker";

        (int PageId, int ComponentId) seeded = await SeedComponentPageAsync(
            componentName: componentName,
            pagePath: pagePath,
            marker: originalMarker);

        PageRenderResponse firstResponse = await RenderAsync(path: pagePath);

        PageRenderCache firstCache = await GetCacheAsync(
            pageId: seeded.PageId);

        // When
        using HttpResponseMessage updateResponse =
            await fixture.Client.PutAsJsonAsync(
                requestUri:
                    $"/Api/ContentManagement/Component({seeded.ComponentId})",
                value: new
                {
                    id = seeded.ComponentId,
                    appId = 1,
                    name = componentName,
                    description = "Lifecycle component updated",
                    resourceKey = "Default",
                    content = updatedMarker,
                    script = string.Empty,
                    key = "Acceptance"
                });

        string updateContent = await updateResponse.Content
            .ReadAsStringAsync();

        Component storedComponent = await GetComponentAsync(
            componentId: seeded.ComponentId);

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

        storedComponent.Content.Should()
            .Be(expected: updatedMarker);

        cachesAfterUpdate.Should()
            .BeEmpty(
                because: "an app component update must invalidate rendered variants for its app");

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

    private async Task<(int PageId, int ComponentId)> SeedComponentPageAsync(
        string componentName,
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
            body: $"<main>[component[{componentName}]]</main>",
            timestamp: now);

        Page page = CreatePage(
            name: $"LifecyclePage{Guid.NewGuid():N}",
            path: pagePath,
            layoutName: layout.Name,
            timestamp: now);

        Component component = new()
        {
            AppId = 1,
            Name = componentName,
            Description = "Lifecycle component",
            ResourceKey = "Default",
            Content = marker,
            Script = string.Empty,
            Key = "Acceptance",
            CreatedOn = now,
            CreatedBy = "Guest",
            LastUpdated = now,
            LastUpdatedBy = "Guest"
        };

        core.AddRange(entities: [layout, page, component]);
        await core.SaveChangesAsync();

        return (PageId: page.Id, ComponentId: component.Id);
    }

    private async Task<PageRenderResponse> RenderAsync(string path)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        IHttpContextAccessor accessor = scope.ServiceProvider
            .GetRequiredService<IHttpContextAccessor>();

        DefaultHttpContext context = new()
        {
            RequestServices = scope.ServiceProvider
        };

        context.Request.Host = new HostString(value: "localhost");
        context.Request.Path = new PathString(value: $"/{path}");

        context.Request.QueryString = new QueryString(
            value: "?culture=&theme=Default");

        context.Items[ContentSecurityPolicyNonceContract.HttpContextItemKey] =
            "acceptance-request-nonce";

        accessor.HttpContext = context;

        try
        {
            IPageRenderer renderer = scope.ServiceProvider
                .GetRequiredService<IPageRenderer>();

            return await renderer.RenderAsync();
        }
        finally
        {
            accessor.HttpContext = null;
        }
    }

    private async Task<Component> GetComponentAsync(int componentId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await core.Components
            .IgnoreQueryFilters()
            .SingleAsync(predicate: item => item.Id == componentId);
    }

    private async Task<PageRenderCache> GetCacheAsync(int pageId) =>
        (await GetCachesAsync(pageId: pageId)).Single();

    private async Task<PageRenderCache[]> GetCachesAsync(int pageId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await core.PageRenderCaches
            .IgnoreQueryFilters()
            .Where(predicate: item => item.PageId == pageId)
            .ToArrayAsync();
    }

    private static Layout CreateLayout(
        string name,
        string body,
        DateTimeOffset timestamp) =>
        new()
        {
            AppId = 1,
            Name = name,
            Description = "Lifecycle layout",
            HeaderHtml = string.Empty,
            Html = body,
            Script = string.Empty,
            CreatedOn = timestamp,
            CreatedBy = "Guest",
            LastUpdated = timestamp,
            LastUpdatedBy = "Guest"
        };

    private static Page CreatePage(
        string name,
        string path,
        string layoutName,
        DateTimeOffset timestamp) =>
        new()
        {
            AppId = 1,
            Name = name,
            Path = path,
            Layout = layoutName,
            ResourceKey = "Default",
            ShowOnMenus = false,
            CreatedOn = timestamp,
            CreatedBy = "Guest",
            LastUpdated = timestamp,
            LastUpdatedBy = "Guest"
        };
}