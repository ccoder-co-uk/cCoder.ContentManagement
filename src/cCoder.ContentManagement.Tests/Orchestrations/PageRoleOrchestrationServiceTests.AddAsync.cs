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

public partial class PageRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        PageRole entity = CreateRandomPageRole();
        pageRoleProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        pageRoleEventProcessingServiceMock
            .Setup(x => x.RaisePageRoleAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        PageRole result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        pageRoleProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        pageRoleEventProcessingServiceMock.Verify(x => x.RaisePageRoleAddEventAsync(entity), Times.Once);
    }

}


















