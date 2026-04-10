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

public partial class TemplateOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();
        templateProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        templateEventProcessingServiceMock
            .Setup(x => x.RaiseTemplateUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Template result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        templateProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        templateEventProcessingServiceMock.Verify(x => x.RaiseTemplateUpdateEventAsync(entity), Times.Once);
    }

}




















