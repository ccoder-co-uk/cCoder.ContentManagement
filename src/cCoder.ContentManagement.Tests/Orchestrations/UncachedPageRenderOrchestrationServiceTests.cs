// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed partial class UncachedPageRenderOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeferPageNotFoundOutcomeUntilEventCompletesAsync()
    {
        // Given
        HttpPageRenderOperation operation = new()
        {
            Context = new HttpPageRenderContext()
        };

        UncachedPageRenderOrchestrationService service = new(
            pageProcessingService: Mock.Of<IPageProcessingService>(),
            pageRenderProcessingService:
                Mock.Of<IPageRenderProcessingService>());

        // When
        Exception exception = await Record.ExceptionAsync(
            testCode: async () =>
                await service.PrepareHttpPageRenderOperationAsync(
                    httpPageRenderOperation: operation));

        // Then
        Assert.Null(@object: exception);
        Assert.Null(@object: operation.RenderOperation);

        Assert.Equal(
            expected: HttpPageRenderFailure.PageNotFound,
            actual: operation.Failure);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShouldRenderOnlyRequestedVariantAsync(bool edit)
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

        renderService.Setup(expression: service =>
            service.CompletePageRenderOperation(
                operation: It.IsAny<PageRenderOperation>()))
            .Returns(valueFunction: (PageRenderOperation item) => item);

        UncachedPageRenderOrchestrationService service = new(
            pageProcessingService: pageService.Object,
            pageRenderProcessingService: renderService.Object);

        // When
        await service.PrepareHttpPageRenderOperationAsync(
            httpPageRenderOperation: operation);

        await service.CompleteHttpPageRenderOperationAsync(
            httpPageRenderOperation: operation);

        // Then
        Assert.NotNull(@object: operation.Response);
        Assert.Equal(expected: edit, actual: operation.Response.Edit);
        pageService.VerifyAll();
        renderService.VerifyAll();
    }
}