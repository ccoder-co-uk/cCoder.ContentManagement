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
}
