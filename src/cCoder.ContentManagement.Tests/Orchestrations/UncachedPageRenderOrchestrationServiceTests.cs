// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed partial class UncachedPageRenderOrchestrationServiceTests
{
    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task ShouldRenderAndStoreOnlyRequestedVariantOutsideEditModeAsync(
        bool edit,
        bool shouldStore)
    {
        // Given
        App app = new()
        {
            Id = 3,
            DefaultCultureId = "en",
            DefaultTheme = "Default"
        };

        Page page = new() { Id = 17, App = app };
        User user = new() { Id = "Paul" };

        HttpPageRenderOperation operation = new()
        {
            Context = new HttpPageRenderContext
            {
                PageId = page.Id,
                Edit = edit,
                User = user,
                Culture = "en-gb",
                Theme = "dark"
            }
        };

        Mock<IPageProcessingService> pageService = new();
        Mock<IPageRenderProcessingService> renderService = new();
        Mock<IPageRenderCacheProcessingService> cacheService = new();

        pageService.Setup(expression: service =>
            service.GetPageForRenderAsync(pageId: page.Id))
            .ReturnsAsync(value: page);

        renderService.Setup(expression: service =>
            service.RenderPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: item =>
                    item.SourcePage == page &&
                    item.User == user &&
                    item.Edit == edit)))
            .Returns(valueFunction: (PageRenderOperation item) =>
            {
                item.Page = new PageRenderResult
                {
                    AppId = app.Id,
                    PageId = page.Id,
                    Path = "Admin/AppManagement",
                    HeaderHtml = "header",
                    BodyHtml = "body"
                };

                return item;
            });

        if (shouldStore)
        {
            cacheService.Setup(expression: service =>
                service.StorePageRenderCacheAsync(
                    pageRenderCache: It.Is<PageRenderCache>(match:
                        item => item.AppId == app.Id
                            && item.PageId == page.Id
                            && item.Culture == "en-gb"
                            && item.Theme == "dark"
                            && item.Path == "Admin/AppManagement"
                            && item.Header == "header"
                            && item.Body == "body")))
                .ReturnsAsync(valueFunction: (PageRenderCache item) => item);
        }

        UncachedPageRenderOrchestrationService service = new(
            pageProcessingService: pageService.Object,
            pageRenderProcessingService: renderService.Object,
            pageRenderCacheProcessingService: cacheService.Object);

        // When
        HttpPageRenderOperation result = await service
            .RenderHttpPageRenderOperationAsync(operation: operation);

        // Then
        Assert.NotNull(@object: result.Response);
        Assert.Equal(expected: edit, actual: result.Response.Edit);
        pageService.VerifyAll();
        renderService.VerifyAll();

        if (shouldStore)
        {
            cacheService.VerifyAll();
        }
        else
        {
            cacheService.VerifyNoOtherCalls();
        }
    }
}