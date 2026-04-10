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

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Page entity = CreateRandomPage();
        entity.Layout = "Default";
        entity.PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Home" }];
        entity.Contents = [];
        pageProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        pageEventProcessingServiceMock
            .Setup(x => x.RaisePageAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Page result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        pageProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        pageEventProcessingServiceMock.Verify(x => x.RaisePageAddEventAsync(entity), Times.Once);
    }

}




















