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

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();
        layoutProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        layoutEventProcessingServiceMock
            .Setup(x => x.RaiseLayoutUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Layout result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        layoutProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        layoutEventProcessingServiceMock.Verify(x => x.RaiseLayoutUpdateEventAsync(entity), Times.Once);
    }

}




















