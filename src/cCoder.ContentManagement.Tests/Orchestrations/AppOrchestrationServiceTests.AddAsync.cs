using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        App entity = CreateRandomApp();
        appProcessingServiceMock
            .Setup(x => x.AddAppAsync(entity))
            .ReturnsAsync((App app) => app);
        appEventProcessingServiceMock
            .Setup(x => x.RaiseAppAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        App result = await orchestrationService.AddAppAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        appProcessingServiceMock.Verify(x => x.AddAppAsync(It.IsAny<App>()), Times.Once);
        appEventProcessingServiceMock.Verify(x => x.RaiseAppAddEventAsync(entity), Times.Once);
        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















