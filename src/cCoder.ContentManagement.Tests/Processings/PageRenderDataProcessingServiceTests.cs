// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class PageRenderDataProcessingServiceTests
{
    [Fact]
    public async Task GetPageForRender_WhenRenderDataExists_AttachesAppCollectionsAsync()
    {
        // Given
        const int pageId = 17;
        App app = new() { Id = 3 };
        Page page = new() { Id = pageId, AppId = app.Id, App = app };
        Layout[] layouts = [new Layout()];
        Template[] templates = [new Template()];
        Resource[] resources = [new Resource()];
        Component[] components = [new Component()];
        Script[] scripts = [new Script()];
        Page[] pages = [page];

        PageRenderData renderData = new()
        {
            Page = page,
            Layouts = layouts,
            Templates = templates,
            Resources = resources,
            Components = components,
            Scripts = scripts,
            Pages = pages
        };

        Mock<IPageRenderDataService> serviceMock = new(
            behavior: MockBehavior.Strict);

        serviceMock.Setup(expression: service =>
            service.GetPageRenderDataAsync(pageId: pageId))
            .ReturnsAsync(value: renderData);

        PageRenderDataProcessingService service = new(
            service: serviceMock.Object);

        // When
        Page result = await service.GetPageForRenderAsync(pageId: pageId);

        // Then
        Assert.Same(expected: page, actual: result);
        Assert.Same(expected: layouts, actual: result.App.Layouts);
        Assert.Same(expected: templates, actual: result.App.Templates);
        Assert.Same(expected: resources, actual: result.App.Resources);
        Assert.Same(expected: components, actual: result.App.Components);
        Assert.Same(expected: scripts, actual: result.App.Scripts);
        Assert.Same(expected: pages, actual: result.App.Pages);
        serviceMock.VerifyAll();
    }

    [Fact]
    public async Task GetPageForRender_WhenPageDoesNotExist_ReturnsNullAsync()
    {
        // Given
        const int pageId = 17;

        PageRenderData renderData = new()
        {
            Page = null,
            Layouts = [],
            Templates = [],
            Resources = [],
            Components = [],
            Scripts = [],
            Pages = []
        };

        Mock<IPageRenderDataService> serviceMock = new(
            behavior: MockBehavior.Strict);

        serviceMock.Setup(expression: service =>
            service.GetPageRenderDataAsync(pageId: pageId))
            .ReturnsAsync(value: renderData);

        PageRenderDataProcessingService service = new(
            service: serviceMock.Object);

        // When
        Page result = await service.GetPageForRenderAsync(pageId: pageId);

        // Then
        Assert.Null(@object: result);
        serviceMock.VerifyAll();
    }
}