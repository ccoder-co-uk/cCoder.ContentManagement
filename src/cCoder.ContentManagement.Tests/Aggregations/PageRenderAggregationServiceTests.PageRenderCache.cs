// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Aggregations;

public partial class PageRenderAggregationServiceTests
{
    [Fact]
    public async Task ShouldReturnCachedRenderAfterEstablishedHydrationAndAuthorizationAsync()
    {
        // Given
        App app = CreateApp();
        Page page = SetupCacheablePage(app: app);
        currentUser = TestUsers.WithPrivilege(privilege: "app_admin", appId: app.Id);
        RenderResult cachedResult = CreateRenderResult(bodyHtml: "Cached Body");
        cachedResult.AppId = app.Id;
        cachedResult.PageId = page.Id;
        SetupRenderResult(renderResult: cachedResult);

        cachedPageRenderOrchestrationServiceMock
            .Setup(expression: service => service
                .RenderCachedPageRenderOperationAsync(
                    operation: It.IsAny<CachedPageRenderOperation>()))
            .ReturnsAsync(valueFunction: (CachedPageRenderOperation operation) =>
            {
                operation.RenderResult = cachedResult;
                return operation;
            });

        // When
        PageRenderOperation operation = await aggregationService
            .RenderPageRenderOperationAsync(
            operation: new PageRenderOperation
            {
                OperationType = PageRenderOperationType.RenderResult,
                AppId = app.Id,
                Path = page.Path,
                Theme = app.DefaultTheme,
                Culture = app.DefaultCultureId
            });

        // Then
        RenderResult result = operation.Page as RenderResult;

        result.Should()
            .BeEquivalentTo(expectation: cachedResult);

        cachedPageRenderOrchestrationServiceMock.Verify(
            expression: service => service
                .RenderCachedPageRenderOperationAsync(
                    operation: It.IsAny<CachedPageRenderOperation>()),
            times: Times.Once());

        VerifyRenderDependencies(times: Times.Once());
    }

    [Fact]
    public void ShouldIgnoreCacheForEditRender()
    {
        // Given
        App app = CreateApp();
        Page page = SetupCacheablePage(app: app);
        currentUser = TestUsers.WithPrivilege(privilege: "app_admin", appId: app.Id);
        RenderResult rendered = CreateRenderResult(bodyHtml: "Edited Body");
        SetupRenderResult(renderResult: rendered);

        // When
        RenderResult result = aggregationService.RenderRenderResult(
            appId: app.Id,
            path: page.Path,
            theme: app.DefaultTheme,
            culture: app.DefaultCultureId,
            edit: true);

        // Then
        result.Should()
            .BeSameAs(expected: rendered);

        pageRenderCacheOrchestrationServiceMock.Verify(
            expression: service => service.GetAllPageRenderCaches(),
            times: Times.Never());
    }

    [Fact]
    public void ShouldIgnoreCacheForDeniedRender()
    {
        // Given
        App app = CreateApp();
        Page page = SetupCacheablePage(app: app);
        currentUser = TestUsers.WithoutPrivileges();
        RenderResult rendered = CreateRenderResult(bodyHtml: "Login Required");
        SetupRenderResult(renderResult: rendered);

        // When
        RenderResult result = aggregationService.RenderRenderResult(
            appId: app.Id,
            path: page.Path,
            theme: app.DefaultTheme,
            culture: app.DefaultCultureId);

        // Then
        result.Should()
            .BeSameAs(expected: rendered);

        pageRenderCacheOrchestrationServiceMock.Verify(
            expression: service => service.GetAllPageRenderCaches(),
            times: Times.Never());
    }

    [Fact]
    public void ShouldUseExistingRendererForSynchronousRender()
    {
        // Given
        App app = CreateApp();
        Page page = SetupCacheablePage(app: app);
        currentUser = TestUsers.WithPrivilege(privilege: "app_admin", appId: app.Id);
        RenderResult rendered = CreateRenderResult(bodyHtml: "Fresh Body");
        SetupRenderResult(renderResult: rendered);

        // When
        RenderResult result = aggregationService.RenderRenderResult(
            appId: app.Id,
            path: page.Path,
            theme: app.DefaultTheme,
            culture: app.DefaultCultureId);

        // Then
        result.Should()
            .BeSameAs(expected: rendered);

        pageRenderCacheOrchestrationServiceMock.Verify(
            expression: service => service.GetAllPageRenderCaches(),
            times: Times.Never());

        VerifyRenderDependencies(times: Times.Once());
    }

    [Fact]
    public async Task ShouldRebuildEveryConfiguredPageVariantBeforeReplacingCacheAsync()
    {
        // Given
        App app = CreateApp();
        app.ConfigJson = "{\"Themes\":{\"Ocean\":{},\"Dark\":{}}}";
        Page page = SetupCacheablePage(app: app);
        currentUser = TestUsers.WithPrivilege(privilege: "app_admin", appId: app.Id);
        RenderResult rendered = CreateRenderResult(bodyHtml: "Fresh Body");
        SetupRenderResult(renderResult: rendered);

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetApp(appId: app.Id))
            .Returns(value: app);

        pageOrchestrationServiceMock
            .Setup(expression: service => service.GetPage(pageId: page.Id))
            .Returns(value: page);

        appCultureOrchestrationServiceMock
            .Setup(expression: service => service.GetAllAppCulture(
                ignoreFilters: true))
            .Returns(value: new[]
            {
                new AppCulture { AppId = app.Id, CultureId = "en-GB" },
                new AppCulture { AppId = app.Id, CultureId = "fr-FR" }
            }.AsQueryable());

        PageRenderCache[] replacements = null;

        pageRenderCacheOrchestrationServiceMock
            .Setup(expression: service => service.ReplacePageRenderCachesAsync(
                appId: app.Id,
                pageIds: It.Is<int[]>(match: pageIds => pageIds
                    .SequenceEqual(second: new[] { page.Id })),
                replacements: It.IsAny<PageRenderCache[]>()))
            .Callback<int, int[], PageRenderCache[]>(action: (_, _, caches) =>
                replacements = caches)
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageRenderOperation result = await aggregationService
            .RebuildPagePageRenderOperationAsync(
                operation: new PageRenderOperation { PageId = page.Id });

        // Then
        result.PageRenderCaches.Should()
            .HaveCount(expected: 6);

        replacements.Should()
            .BeEquivalentTo(expectation: result.PageRenderCaches);

        result.PageRenderCaches
            .Select(selector: cache => (cache.Culture, cache.Theme))
            .Should()
            .BeEquivalentTo(expectation: new[]
            {
                (string.Empty, "ocean"),
                (string.Empty, "dark"),
                ("en-gb", "ocean"),
                ("en-gb", "dark"),
                ("fr-fr", "ocean"),
                ("fr-fr", "dark")
            });

        result.PageRenderCaches.Should()
            .OnlyContain(predicate: cache =>
                cache.Header == rendered.HeaderHtml
                && !string.IsNullOrWhiteSpace(value: cache.Body)
                && cache.SourceFingerprint != null
                && cache.SourceFingerprint.Length == 64);

        pageRenderCacheOrchestrationServiceMock.Verify(
            expression: service => service.ReplacePageRenderCachesAsync(
                appId: app.Id,
                pageIds: It.IsAny<int[]>(),
                replacements: It.IsAny<PageRenderCache[]>()),
            times: Times.Once());
    }

    [Fact]
    public async Task ShouldPreserveExistingCacheWhenRebuildRenderingFailsAsync()
    {
        // Given
        App app = CreateApp();
        Page page = SetupCacheablePage(app: app);
        currentUser = TestUsers.WithPrivilege(privilege: "app_admin", appId: app.Id);

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetApp(appId: app.Id))
            .Returns(value: app);

        pageOrchestrationServiceMock
            .Setup(expression: service => service.GetPage(pageId: page.Id))
            .Returns(value: page);

        appCultureOrchestrationServiceMock
            .Setup(expression: service => service.GetAllAppCulture(
                ignoreFilters: true))
            .Throws(exception: new InvalidOperationException(message: "render failed"));

        // When
        Func<Task> rebuild = async () => await aggregationService
            .RebuildPagePageRenderOperationAsync(
                operation: new PageRenderOperation { PageId = page.Id });

        // Then
        await rebuild.Should()
            .ThrowAsync<Exception>();

        pageRenderCacheOrchestrationServiceMock.Verify(
            expression: service => service.ReplacePageRenderCachesAsync(
                appId: It.IsAny<int>(),
                pageIds: It.IsAny<int[]>(),
                replacements: It.IsAny<PageRenderCache[]>()),
            times: Times.Never());
    }

    [Fact]
    public async Task ShouldRebuildEveryPageAndCultureVariantForAppAsync()
    {
        // Given
        App app = CreateApp();
        Page firstPage = SetupCacheablePage(app: app);

        Page secondPage = new()
        {
            Id = 11,
            AppId = app.Id,
            Name = "Details",
            Path = "Details",
            App = app,
            PageInfo = [],
            Contents = [new Content { Name = "Body", Html = "Details" }],
            Roles = []
        };

        currentUser = TestUsers.WithPrivilege(
            privilege: "app_admin",
            appId: app.Id);

        SetupRenderResult(renderResult: CreateRenderResult());

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetApp(appId: app.Id))
            .Returns(value: app);

        pageOrchestrationServiceMock
            .Setup(expression: service => service.GetAllPage(
                ignoreFilters: true))
            .Returns(value: new[] { firstPage, secondPage }.AsQueryable());

        appCultureOrchestrationServiceMock
            .Setup(expression: service => service.GetAllAppCulture(
                ignoreFilters: true))
            .Returns(value: new[]
            {
                new AppCulture { AppId = app.Id, CultureId = "en-GB" }
            }.AsQueryable());

        pageRenderCacheOrchestrationServiceMock
            .Setup(expression: service => service.ReplacePageRenderCachesAsync(
                appId: app.Id,
                pageIds: It.IsAny<int[]>(),
                replacements: It.IsAny<PageRenderCache[]>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageRenderOperation result = await aggregationService
            .RebuildAppPageRenderOperationAsync(
                operation: new PageRenderOperation { AppId = app.Id });

        // Then
        result.PageRenderCaches.Should()
            .HaveCount(expected: 4);

        result.PageRenderCaches
            .Select(selector: cache => (cache.PageId, cache.Culture))
            .Should()
            .BeEquivalentTo(expectation: new[]
            {
                (firstPage.Id, string.Empty),
                (firstPage.Id, "en-gb"),
                (secondPage.Id, string.Empty),
                (secondPage.Id, "en-gb")
            });

        pageRenderCacheOrchestrationServiceMock.Verify(
            expression: service => service.ReplacePageRenderCachesAsync(
                appId: app.Id,
                pageIds: It.Is<int[]>(match: pageIds =>
                    pageIds.OrderBy(keySelector: pageId => pageId)
                        .SequenceEqual(
                        second: new[] { firstPage.Id, secondPage.Id })),
                replacements: It.Is<PageRenderCache[]>(match: caches =>
                    caches.Length == 4)),
            times: Times.Once());
    }

    [Theory]
    [InlineData("ContentManagement/Component")]
    [InlineData("ContentManagement/Resource")]
    [InlineData("ContentManagement/Script")]
    public async Task ShouldRebuildEveryAppConsumingCommonCacheRenderObjectAsync(
        string commonObjectType)
    {
        // Given
        App firstApp = CreateApp();
        App secondApp = CreateApp();
        secondApp.Id = 2;
        secondApp.Domain = "second.demo.local";

        Page firstPage = SetupCacheablePage(app: firstApp);
        Page secondPage = SetupCacheablePage(app: secondApp);
        secondPage.Id = 20;

        currentUser = TestUsers.WithPrivilege(
            privilege: "app_admin",
            appId: firstApp.Id);

        SetupRenderResult(renderResult: CreateRenderResult());

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetAllApp(
                ignoreFilters: true))
            .Returns(value: new[] { firstApp, secondApp }.AsQueryable());

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetAllApp(
                ignoreFilters: false))
            .Returns(value: new[] { firstApp, secondApp }.AsQueryable());

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetApp(
                appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) =>
                appId == firstApp.Id ? firstApp : secondApp);

        pageOrchestrationServiceMock
            .Setup(expression: service => service.GetAllPage(
                ignoreFilters: true))
            .Returns(value: new[] { firstPage, secondPage }.AsQueryable());

        appCultureOrchestrationServiceMock
            .Setup(expression: service => service.GetAllAppCulture(
                ignoreFilters: true))
            .Returns(value: Array.Empty<AppCulture>()
                .AsQueryable());

        pageRenderCacheOrchestrationServiceMock
            .Setup(expression: service => service.ReplacePageRenderCachesAsync(
                appId: It.IsAny<int>(),
                pageIds: It.IsAny<int[]>(),
                replacements: It.IsAny<PageRenderCache[]>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageRenderOperation result = await aggregationService
            .RebuildCommonObjectPageRenderOperationAsync(
                operation: new PageRenderOperation
                {
                    CommonObject = new CommonObject
                    {
                        Type = commonObjectType
                    }
                });

        // Then
        result.PageRenderCaches.Should()
            .HaveCount(expected: 2);

        pageRenderCacheOrchestrationServiceMock.Verify(
            expression: service => service.ReplacePageRenderCachesAsync(
                appId: It.IsAny<int>(),
                pageIds: It.IsAny<int[]>(),
                replacements: It.IsAny<PageRenderCache[]>()),
            times: Times.Exactly(callCount: 2));
    }

    private Page SetupCacheablePage(App app)
    {
        Page page = new()
        {
            Id = 10,
            AppId = app.Id,
            Name = "Summary",
            Path = "Summary",
            App = app,
            PageInfo = [],
            Contents = [new Content { Name = "Body", Html = "Rendered Body" }],
            Roles = []
        };

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        appOrchestrationServiceMock
            .Setup(expression: service => service.GetAllApp(
                ignoreFilters: true))
            .Returns(value: new[] { app }.AsQueryable());

        pageOrchestrationServiceMock
            .Setup(expression: service => service.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        pageOrchestrationServiceMock
            .Setup(expression: service => service.GetAllPage())
            .Returns(value: new[] { page }.AsQueryable());

        layoutOrchestrationServiceMock
            .Setup(expression: service => service.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        return page;
    }

    private void VerifyRenderDependencies(Times times)
    {
        layoutOrchestrationServiceMock.Verify(
            expression: service => service.GetAllLayout(),
            times: times);

        templateOrchestrationServiceMock.Verify(
            expression: service => service.GetAllTemplate(),
            times: times);

        resourceOrchestrationServiceMock.Verify(
            expression: service => service.GetAllResource(),
            times: times);

        componentOrchestrationServiceMock.Verify(
            expression: service => service.GetAllComponent(),
            times: times);

        scriptOrchestrationServiceMock.Verify(
            expression: service => service.GetAllScript(),
            times: times);
    }
}