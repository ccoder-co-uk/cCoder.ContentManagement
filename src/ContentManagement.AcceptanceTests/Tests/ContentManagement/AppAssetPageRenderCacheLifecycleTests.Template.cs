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
    public async Task TemplateUpdate_InvalidatesAppThenRebuildsChangedPageAsync()
    {
        // Given
        string suffix = Guid.NewGuid().ToString(format: "N");
        string pagePath = $"lifecycle-template-{suffix}";
        const string originalMarker = "original-app-template-marker";
        const string updatedMarker = "updated-app-template-marker";

        (int PageId, int TemplateId) seeded = await SeedTemplatePageAsync(
            pagePath: pagePath,
            marker: originalMarker);

        PageRenderResponse firstResponse = await RenderAsync(path: pagePath);
        PageRenderCache firstCache = await GetCacheAsync(
            pageId: seeded.PageId);

        // When
        using HttpResponseMessage updateResponse =
            await fixture.Client.PutAsJsonAsync(
                requestUri:
                    $"/Api/ContentManagement/Template({seeded.TemplateId})",
                value: new
                {
                    id = seeded.TemplateId,
                    appId = 1,
                    name = "Theme",
                    description = "Lifecycle theme template updated",
                    resourceKey = "Default",
                    rawString = updatedMarker
                });

        string updateContent = await updateResponse.Content
            .ReadAsStringAsync();

        Template storedTemplate = await GetTemplateAsync(
            templateId: seeded.TemplateId);

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

        storedTemplate.RawString.Should()
            .Be(expected: updatedMarker);

        cachesAfterUpdate.Should()
            .BeEmpty(
                because: "an app template update must invalidate rendered variants for its app");

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

    private async Task<(int PageId, int TemplateId)> SeedTemplatePageAsync(
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
            body: "<main>[theme[base]]</main>",
            timestamp: now);

        Page page = CreatePage(
            name: $"LifecyclePage{Guid.NewGuid():N}",
            path: pagePath,
            layoutName: layout.Name,
            timestamp: now);

        Template template = await core.Templates
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(predicate: item =>
                item.AppId == 1 && item.Name == "Theme");

        if (template is null)
        {
            template = new Template
            {
                AppId = 1,
                Name = "Theme",
                Description = "Lifecycle theme template",
                ResourceKey = "Default",
                CreatedOn = now,
                CreatedBy = "Guest"
            };

            core.Add(entity: template);
        }

        template.RawString = marker;
        template.LastUpdated = now;
        template.LastUpdatedBy = "Guest";

        core.AddRange(layout, page);
        await core.SaveChangesAsync();

        return (PageId: page.Id, TemplateId: template.Id);
    }

    private async Task<Template> GetTemplateAsync(int templateId)
    {
        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        await using CoreDataContext core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        return await core.Templates
            .IgnoreQueryFilters()
            .SingleAsync(predicate: item => item.Id == templateId);
    }
}
