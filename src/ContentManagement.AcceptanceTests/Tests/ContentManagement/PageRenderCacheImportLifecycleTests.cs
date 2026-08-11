// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Models;
using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class PageRenderCacheImportLifecycleTests(WebAcceptanceFixture fixture)
{
    [Fact]
    public async Task CacheMiss_RendersCurrentCommonComponentThenCachesExactResult()
    {
        // Given

        string suffix = Guid.NewGuid()
            .ToString(format: "N");

        string componentName = $"AcceptanceComponent{suffix}";
        string commonObjectName = $"AcceptanceObject{suffix}";
        string commonObjectKey = $"AcceptanceKey{suffix}";
        string pagePath = $"acceptance-cache-{suffix}";
        const string originalMarker = "original-common-component-marker";
        const string importedMarker = "imported-common-component-marker";

        int pageId = await SeedRenderGraphAsync(
            componentName: componentName,
            commonObjectName: commonObjectName,
            commonObjectKey: commonObjectKey,
            pagePath: pagePath,
            marker: originalMarker);

        ICommonObjectCache commonObjectCache = fixture.Factory.Services
            .GetRequiredService<ICommonObjectCache>();

        commonObjectCache.Refresh();

        PageRenderResponse firstResponse = await RenderAsync(path: pagePath);
        PageRenderCache firstCache = await GetCacheAsync(pageId: pageId);

        await ImportAsync(
            commonObjectName: commonObjectName,
            commonObjectKey: commonObjectKey,
            componentName: componentName,
            marker: importedMarker);

        PageRenderCache[] cachesAfterImport =
            await GetCachesAsync(pageId: pageId);

        // When
        PageRenderResponse refreshedResponse = await RenderAsync(path: pagePath);
        PageRenderCache refreshedCache = await GetCacheAsync(pageId: pageId);
        PageRenderResponse cachedResponse = await RenderAsync(path: pagePath);
        PageRenderCache cachedAgain = await GetCacheAsync(pageId: pageId);

        // Then
        firstResponse.Page.BodyHtml.Should()
            .Contain(expected: originalMarker);

        firstCache.Body.Should()
            .Contain(expected: originalMarker);

        cachesAfterImport.Should()
            .BeEmpty(
                because: "a Common Cache import invalidates every rendered page");

        refreshedResponse.Page.BodyHtml.Should()
            .Contain(expected: importedMarker);

        refreshedCache.Body.Should()
            .Contain(expected: importedMarker);

        cachedResponse.Page.BodyHtml.Should()
            .Be(expected: refreshedResponse.Page.BodyHtml);

        cachedAgain.Id.Should()
            .Be(expected: refreshedCache.Id);

        cachedAgain.Body.Should()
            .Be(expected: refreshedCache.Body);

        cachedAgain.RenderedOn.Should()
            .Be(expected: refreshedCache.RenderedOn);
    }

    private async Task<int> SeedRenderGraphAsync(
        string componentName,
        string commonObjectName,
        string commonObjectKey,
        string pagePath,
        string marker)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        DateTimeOffset now = DateTimeOffset.UtcNow.AddMinutes(minutes: -2);

        App app = await core.Apps
            .IgnoreQueryFilters()
            .SingleAsync(predicate: item => item.Id == 1);

        Layout layout = new()
        {
            AppId = app.Id,
            Name = $"AcceptanceLayout{Guid.NewGuid():N}",
            Description = "Acceptance render-cache lifecycle layout",
            HeaderHtml = string.Empty,
            Html = $"<main>[component[{componentName}]]</main>",
            Script = string.Empty,
            CreatedOn = now,
            CreatedBy = "Guest",
            LastUpdated = now,
            LastUpdatedBy = "Guest"
        };

        Page page = new()
        {
            AppId = app.Id,
            Name = $"AcceptancePage{Guid.NewGuid():N}",
            Path = pagePath,
            Layout = layout.Name,
            ResourceKey = "Default",
            ShowOnMenus = false,
            CreatedOn = now,
            CreatedBy = "Guest",
            LastUpdated = now,
            LastUpdatedBy = "Guest"
        };

        core.AddRange(
            entities:
            [
                layout,
                page,
            CreateCommonObject(
                commonObjectName: commonObjectName,
                commonObjectKey: commonObjectKey,
                componentName: componentName,
                marker: marker,
                timestamp: now)
            ]);

        await core.SaveChangesAsync();
        return page.Id;
    }

    private async Task ImportAsync(
        string commonObjectName,
        string commonObjectKey,
        string componentName,
        string marker)
    {
        DateTimeOffset timestamp = DateTimeOffset.UtcNow;

        CommonObject commonObject = CreateCommonObject(
            commonObjectName: commonObjectName,
            commonObjectKey: commonObjectKey,
            componentName: componentName,
            marker: marker,
            timestamp: timestamp);

        using HttpResponseMessage response = await fixture.Client.PostAsJsonAsync(
            requestUri: "/Api/ContentManagement/CommonObject",
            value: new { value = new[] { commonObject } });

        string content = await response.Content.ReadAsStringAsync();

        response.IsSuccessStatusCode.Should()
            .BeTrue(because: content);
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

        context.Request.QueryString = new QueryString(value: "?culture=&theme=Default");

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

    private async Task<PageRenderCache> GetCacheAsync(int pageId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await core.PageRenderCaches
            .IgnoreQueryFilters()
            .SingleAsync(predicate: item => item.PageId == pageId);
    }

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

    private static CommonObject CreateCommonObject(
        string commonObjectName,
        string commonObjectKey,
        string componentName,
        string marker,
        DateTimeOffset timestamp) =>
        new()
        {
            Name = commonObjectName,
            Description = "Acceptance common component",
            LastUpdated = timestamp,
            LastUpdatedBy = "Guest",
            CreatedOn = timestamp,
            CreatedBy = "Guest",
            Version = 1,
            Key = commonObjectKey,
            Type = "ContentManagement/Component",
            Json = "{\"Name\":\"" + componentName
                + "\",\"ResourceKey\":\"Default\",\"Content\":\""
                + marker + "\",\"Script\":\"\"}",
            Culture = string.Empty
        };
}