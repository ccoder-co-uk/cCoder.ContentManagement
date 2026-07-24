// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public class AppRenderableCoordinationServiceTests
{
    private readonly Mock<IPageOrchestrationService> pageOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<IComponentOrchestrationService> componentOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Mock<ILayoutOrchestrationService> layoutOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly AppRenderableCoordinationService coordinationService;

    public AppRenderableCoordinationServiceTests()
    {
        coordinationService = new AppRenderableCoordinationService(
pageOrchestrationService: pageOrchestrationServiceMock.Object,
componentOrchestrationService: componentOrchestrationServiceMock.Object,
templateOrchestrationService: templateOrchestrationServiceMock.Object,
layoutOrchestrationService: layoutOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldAddLayoutsBeforePagesWhenHandleAppAddAsync()
    {
        // Given
        Layout layout = new() { Name = "Default" };
        Page page = new() { Layout = "Default" };

        App app = new()
        {
            Id = 123,
            Layouts = [layout],
            Pages = [page]
        };

        MockSequence sequence = new();

        layoutOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddOrUpdateLayoutResult(
newLayout: It.Is<IEnumerable<Layout>>(match: items => items.Single() == layout && layout.AppId == app.Id)))
            .ReturnsAsync(value: []);

        pageOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddOrUpdatePageResult(
newPage: It.Is<IEnumerable<Page>>(match: items => items.Single() == page && page.AppId == app.Id)))
            .ReturnsAsync(value: []);

        // When
        await coordinationService.HandleAppAddAsync(app: app);

        // Then

        layoutOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdateLayoutResult(newLayout: It.Is<IEnumerable<Layout>>(match: items => items.Single() == layout)),
times: Times.Once);

        pageOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdatePageResult(newPage: It.Is<IEnumerable<Page>>(match: items => items.Single() == page)),
times: Times.Once);

        pageOrchestrationServiceMock.VerifyNoOtherCalls();
        componentOrchestrationServiceMock.VerifyNoOtherCalls();
        templateOrchestrationServiceMock.VerifyNoOtherCalls();
        layoutOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUpdateLayoutsBeforePagesWhenHandleAppUpdateAsync()
    {
        // Given
        Layout layout = new() { Id = 4, Name = "Default" };
        Page page = new() { Id = 1, Layout = "Default" };

        App app = new()
        {
            Id = 123,
            Layouts = [layout],
            Pages = [page]
        };

        MockSequence sequence = new();

        layoutOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.GetAllLayout(ignoreFilters: true))
            .Returns(value: new[] { layout }.AsQueryable());

        layoutOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddOrUpdateLayoutResult(
newLayout: It.Is<IEnumerable<Layout>>(match: items => items.Single() == layout && layout.AppId == app.Id)))
            .ReturnsAsync(value: []);

        pageOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        pageOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddOrUpdatePageResult(
newPage: It.Is<IEnumerable<Page>>(match: items => items.Single() == page && page.AppId == app.Id)))
            .ReturnsAsync(value: []);

        // When
        await coordinationService.HandleAppUpdateAsync(app: app);

        // Then
        layoutOrchestrationServiceMock.Verify(expression: service => service.GetAllLayout(ignoreFilters: true), times: Times.Once);

        layoutOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdateLayoutResult(newLayout: It.Is<IEnumerable<Layout>>(match: items => items.Single() == layout)),
times: Times.Once);

        pageOrchestrationServiceMock.Verify(expression: service => service.GetAllPage(ignoreFilters: true), times: Times.Once);

        pageOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdatePageResult(newPage: It.Is<IEnumerable<Page>>(match: items => items.Single() == page)),
times: Times.Once);

        pageOrchestrationServiceMock.VerifyNoOtherCalls();
        componentOrchestrationServiceMock.VerifyNoOtherCalls();
        templateOrchestrationServiceMock.VerifyNoOtherCalls();
        layoutOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldPassThroughToRenderableOrchestrationsWhenHandleAppDeleteAsync()
    {
        // Given
        App app = new() { Id = 123 };

        pageOrchestrationServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        componentOrchestrationServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        templateOrchestrationServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        layoutOrchestrationServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await coordinationService.HandleAppDeleteAsync(app: app);

        // Then
        pageOrchestrationServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(appId: app.Id), times: Times.Once);
        componentOrchestrationServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(appId: app.Id), times: Times.Once);
        templateOrchestrationServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(appId: app.Id), times: Times.Once);
        layoutOrchestrationServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(appId: app.Id), times: Times.Once);
        pageOrchestrationServiceMock.VerifyNoOtherCalls();
        componentOrchestrationServiceMock.VerifyNoOtherCalls();
        templateOrchestrationServiceMock.VerifyNoOtherCalls();
        layoutOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotTouchRenderableCollectionsWhenHandleAppUpdateAsyncGivenNullCollections()
    {
        // Given
        App app = new()
        {
            Id = 123,
            Pages = null,
            Components = null,
            Templates = null,
            Layouts = null
        };

        // When
        await coordinationService.HandleAppUpdateAsync(app: app);

        // Then
        pageOrchestrationServiceMock.VerifyNoOtherCalls();
        componentOrchestrationServiceMock.VerifyNoOtherCalls();
        templateOrchestrationServiceMock.VerifyNoOtherCalls();
        layoutOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteExistingRenderableCollectionsWhenHandleAppUpdateAsyncGivenEmptyCollections()
    {
        // Given
        App app = new()
        {
            Id = 123,
            Pages = [],
            Components = [],
            Templates = [],
            Layouts = []
        };

        Page existingPage = new() { Id = 1, AppId = app.Id };
        Component existingComponent = new() { Id = 2, AppId = app.Id };
        Template existingTemplate = new() { Id = 3, AppId = app.Id };
        Layout existingLayout = new() { Id = 4, AppId = app.Id };

        pageOrchestrationServiceMock
            .Setup(expression: service => service.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { existingPage }.AsQueryable());

        componentOrchestrationServiceMock
            .Setup(expression: service => service.GetAllComponent(ignoreFilters: true))
            .Returns(value: new[] { existingComponent }.AsQueryable());

        templateOrchestrationServiceMock
            .Setup(expression: service => service.GetAllTemplate(ignoreFilters: true))
            .Returns(value: new[] { existingTemplate }.AsQueryable());

        layoutOrchestrationServiceMock
            .Setup(expression: service => service.GetAllLayout(ignoreFilters: true))
            .Returns(value: new[] { existingLayout }.AsQueryable());

        pageOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllPageAsync(
deletedPage: It.Is<IEnumerable<Page>>(match: items => items.Single() == existingPage)))
            .Returns(value: ValueTask.CompletedTask);

        componentOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllComponentAsync(
deletedComponent: It.Is<IEnumerable<Component>>(match: items => items.Single() == existingComponent)))
            .Returns(value: ValueTask.CompletedTask);

        templateOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllTemplateAsync(
deletedTemplate: It.Is<IEnumerable<Template>>(match: items => items.Single() == existingTemplate)))
            .Returns(value: ValueTask.CompletedTask);

        layoutOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAllLayoutAsync(
deletedLayout: It.Is<IEnumerable<Layout>>(match: items => items.Single() == existingLayout)))
            .Returns(value: ValueTask.CompletedTask);

        pageOrchestrationServiceMock
            .Setup(expression: service => service.AddOrUpdatePageResult(newPage: It.Is<IEnumerable<Page>>(match: items => !items.Any())))
            .ReturnsAsync(value: []);

        templateOrchestrationServiceMock
            .Setup(expression: service => service.AddOrUpdateTemplateResult(newTemplate: It.Is<IEnumerable<Template>>(match: items => !items.Any())))
            .ReturnsAsync(value: []);

        layoutOrchestrationServiceMock
            .Setup(expression: service => service.AddOrUpdateLayoutResult(newLayout: It.Is<IEnumerable<Layout>>(match: items => !items.Any())))
            .ReturnsAsync(value: []);

        // When
        await coordinationService.HandleAppUpdateAsync(app: app);

        // Then
        pageOrchestrationServiceMock.Verify(expression: service => service.GetAllPage(ignoreFilters: true), times: Times.Once);
        componentOrchestrationServiceMock.Verify(expression: service => service.GetAllComponent(ignoreFilters: true), times: Times.Exactly(callCount: 2));
        templateOrchestrationServiceMock.Verify(expression: service => service.GetAllTemplate(ignoreFilters: true), times: Times.Once);
        layoutOrchestrationServiceMock.Verify(expression: service => service.GetAllLayout(ignoreFilters: true), times: Times.Once);

        pageOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllPageAsync(deletedPage: It.Is<IEnumerable<Page>>(match: items => items.Single() == existingPage)),
times: Times.Once);

        componentOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllComponentAsync(deletedComponent: It.Is<IEnumerable<Component>>(match: items => items.Single() == existingComponent)),
times: Times.Once);

        templateOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllTemplateAsync(deletedTemplate: It.Is<IEnumerable<Template>>(match: items => items.Single() == existingTemplate)),
times: Times.Once);

        layoutOrchestrationServiceMock.Verify(
expression: service => service.DeleteAllLayoutAsync(deletedLayout: It.Is<IEnumerable<Layout>>(match: items => items.Single() == existingLayout)),
times: Times.Once);

        pageOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdatePageResult(newPage: It.Is<IEnumerable<Page>>(match: items => !items.Any())),
times: Times.Once);

        templateOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdateTemplateResult(newTemplate: It.Is<IEnumerable<Template>>(match: items => !items.Any())),
times: Times.Once);

        layoutOrchestrationServiceMock.Verify(
expression: service => service.AddOrUpdateLayoutResult(newLayout: It.Is<IEnumerable<Layout>>(match: items => !items.Any())),
times: Times.Once);

        pageOrchestrationServiceMock.VerifyNoOtherCalls();
        componentOrchestrationServiceMock.VerifyNoOtherCalls();
        templateOrchestrationServiceMock.VerifyNoOtherCalls();
        layoutOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}