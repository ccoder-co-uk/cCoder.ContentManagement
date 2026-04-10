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

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Content entity = CreateRandomContent();
        contentProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        contentEventProcessingServiceMock
            .Setup(x => x.RaiseContentAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Content result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        contentProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        contentEventProcessingServiceMock.Verify(x => x.RaiseContentAddEventAsync(entity), Times.Once);
    }

}




















