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

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();
        commonObjectProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        commonObjectEventProcessingServiceMock
            .Setup(x => x.RaiseCommonObjectUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        CommonObject result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        commonObjectProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        commonObjectEventProcessingServiceMock.Verify(x => x.RaiseCommonObjectUpdateEventAsync(entity), Times.Once);
    }

}




















