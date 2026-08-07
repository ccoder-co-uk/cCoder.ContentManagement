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
    public async Task ShouldRenderAndRaiseCacheEventOnlyOutsideEditModeAsync(
        bool edit,
        bool shouldRaiseEvent)
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
                User = user
            }
        };

        Mock<IPageProcessingService> pageService = new();
        Mock<IPageRenderProcessingService> renderService = new();
        Mock<IUncachedPageRenderEventProcessingService> eventService = new();

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
                item.Page = new PageRenderResult();
                return item;
            });

        if (shouldRaiseEvent)
        {
            eventService.Setup(expression: service =>
                service.RaiseUncachedPageRenderEventAsync(
                    pageRenderEvent: It.Is<UncachedPageRenderEvent>(match:
                        item => item.PageId == page.Id)))
                .Returns(value: ValueTask.CompletedTask);
        }

        UncachedPageRenderOrchestrationService service = new(
            pageProcessingService: pageService.Object,
            pageRenderProcessingService: renderService.Object,
            eventProcessingService: eventService.Object);

        // When
        HttpPageRenderOperation result = await service
            .RenderHttpPageRenderOperationAsync(operation: operation);

        // Then
        Assert.NotNull(@object: result.Response);
        Assert.Equal(expected: edit, actual: result.Response.Edit);
        pageService.VerifyAll();
        renderService.VerifyAll();

        if (shouldRaiseEvent)
        {
            eventService.VerifyAll();
        }
        else
        {
            eventService.VerifyNoOtherCalls();
        }
    }
}