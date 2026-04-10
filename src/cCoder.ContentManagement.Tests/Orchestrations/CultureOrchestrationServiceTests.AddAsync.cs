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

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();
        cultureProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        cultureEventProcessingServiceMock
            .Setup(x => x.RaiseCultureAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Culture result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        cultureProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        cultureEventProcessingServiceMock.Verify(x => x.RaiseCultureAddEventAsync(entity), Times.Once);
    }

}





















