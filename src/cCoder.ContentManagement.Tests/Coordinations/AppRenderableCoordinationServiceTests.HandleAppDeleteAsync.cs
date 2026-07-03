using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public class AppRenderableCoordinationServiceTests
{
    private readonly Mock<IPageOrchestrationService> pageOrchestrationServiceMock = new(MockBehavior.Strict);
    private readonly Mock<IComponentOrchestrationService> componentOrchestrationServiceMock = new(MockBehavior.Strict);
    private readonly Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new(MockBehavior.Strict);
    private readonly Mock<ILayoutOrchestrationService> layoutOrchestrationServiceMock = new(MockBehavior.Strict);
    private readonly AppRenderableCoordinationService coordinationService;

    public AppRenderableCoordinationServiceTests()
    {
        coordinationService = new AppRenderableCoordinationService(
            pageOrchestrationServiceMock.Object,
            componentOrchestrationServiceMock.Object,
            templateOrchestrationServiceMock.Object,
            layoutOrchestrationServiceMock.Object);
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
            .InSequence(sequence)
            .Setup(service => service.AddOrUpdate(
                It.Is<IEnumerable<Layout>>(items => items.Single() == layout && layout.AppId == app.Id)))
            .ReturnsAsync([]);
        pageOrchestrationServiceMock
            .InSequence(sequence)
            .Setup(service => service.AddOrUpdate(
                It.Is<IEnumerable<Page>>(items => items.Single() == page && page.AppId == app.Id)))
            .ReturnsAsync([]);

        // When
        await coordinationService.HandleAppAddAsync(app);

        // Then
        layoutOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<Layout>>(items => items.Single() == layout)),
            Times.Once);
        pageOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<Page>>(items => items.Single() == page)),
            Times.Once);
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
            .InSequence(sequence)
            .Setup(service => service.GetAll(true))
            .Returns(new[] { layout }.AsQueryable());
        layoutOrchestrationServiceMock
            .InSequence(sequence)
            .Setup(service => service.AddOrUpdate(
                It.Is<IEnumerable<Layout>>(items => items.Single() == layout && layout.AppId == app.Id)))
            .ReturnsAsync([]);
        pageOrchestrationServiceMock
            .InSequence(sequence)
            .Setup(service => service.GetAll(true))
            .Returns(new[] { page }.AsQueryable());
        pageOrchestrationServiceMock
            .InSequence(sequence)
            .Setup(service => service.AddOrUpdate(
                It.Is<IEnumerable<Page>>(items => items.Single() == page && page.AppId == app.Id)))
            .ReturnsAsync([]);

        // When
        await coordinationService.HandleAppUpdateAsync(app);

        // Then
        layoutOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        layoutOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<Layout>>(items => items.Single() == layout)),
            Times.Once);
        pageOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        pageOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<Page>>(items => items.Single() == page)),
            Times.Once);
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
            .Setup(service => service.DeleteByAppIdAsync(app.Id))
            .Returns(ValueTask.CompletedTask);
        componentOrchestrationServiceMock
            .Setup(service => service.DeleteByAppIdAsync(app.Id))
            .Returns(ValueTask.CompletedTask);
        templateOrchestrationServiceMock
            .Setup(service => service.DeleteByAppIdAsync(app.Id))
            .Returns(ValueTask.CompletedTask);
        layoutOrchestrationServiceMock
            .Setup(service => service.DeleteByAppIdAsync(app.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await coordinationService.HandleAppDeleteAsync(app);

        // Then
        pageOrchestrationServiceMock.Verify(service => service.DeleteByAppIdAsync(app.Id), Times.Once);
        componentOrchestrationServiceMock.Verify(service => service.DeleteByAppIdAsync(app.Id), Times.Once);
        templateOrchestrationServiceMock.Verify(service => service.DeleteByAppIdAsync(app.Id), Times.Once);
        layoutOrchestrationServiceMock.Verify(service => service.DeleteByAppIdAsync(app.Id), Times.Once);
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
        await coordinationService.HandleAppUpdateAsync(app);

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
            .Setup(service => service.GetAll(true))
            .Returns(new[] { existingPage }.AsQueryable());
        componentOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[] { existingComponent }.AsQueryable());
        templateOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[] { existingTemplate }.AsQueryable());
        layoutOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[] { existingLayout }.AsQueryable());
        pageOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(
                It.Is<IEnumerable<Page>>(items => items.Single() == existingPage)))
            .Returns(ValueTask.CompletedTask);
        componentOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(
                It.Is<IEnumerable<Component>>(items => items.Single() == existingComponent)))
            .Returns(ValueTask.CompletedTask);
        templateOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(
                It.Is<IEnumerable<Template>>(items => items.Single() == existingTemplate)))
            .Returns(ValueTask.CompletedTask);
        layoutOrchestrationServiceMock
            .Setup(service => service.DeleteAllAsync(
                It.Is<IEnumerable<Layout>>(items => items.Single() == existingLayout)))
            .Returns(ValueTask.CompletedTask);
        pageOrchestrationServiceMock
            .Setup(service => service.AddOrUpdate(It.Is<IEnumerable<Page>>(items => !items.Any())))
            .ReturnsAsync([]);
        templateOrchestrationServiceMock
            .Setup(service => service.AddOrUpdate(It.Is<IEnumerable<Template>>(items => !items.Any())))
            .ReturnsAsync([]);
        layoutOrchestrationServiceMock
            .Setup(service => service.AddOrUpdate(It.Is<IEnumerable<Layout>>(items => !items.Any())))
            .ReturnsAsync([]);

        // When
        await coordinationService.HandleAppUpdateAsync(app);

        // Then
        pageOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        componentOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Exactly(2));
        templateOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        layoutOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        pageOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(It.Is<IEnumerable<Page>>(items => items.Single() == existingPage)),
            Times.Once);
        componentOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(It.Is<IEnumerable<Component>>(items => items.Single() == existingComponent)),
            Times.Once);
        templateOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(It.Is<IEnumerable<Template>>(items => items.Single() == existingTemplate)),
            Times.Once);
        layoutOrchestrationServiceMock.Verify(
            service => service.DeleteAllAsync(It.Is<IEnumerable<Layout>>(items => items.Single() == existingLayout)),
            Times.Once);
        pageOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<Page>>(items => !items.Any())),
            Times.Once);
        templateOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<Template>>(items => !items.Any())),
            Times.Once);
        layoutOrchestrationServiceMock.Verify(
            service => service.AddOrUpdate(It.Is<IEnumerable<Layout>>(items => !items.Any())),
            Times.Once);
        pageOrchestrationServiceMock.VerifyNoOtherCalls();
        componentOrchestrationServiceMock.VerifyNoOtherCalls();
        templateOrchestrationServiceMock.VerifyNoOtherCalls();
        layoutOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}
