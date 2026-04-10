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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        PageRole pageRole = CreateRandomPageRole();
        pageRoleProcessingServiceMock.Setup(x => x.DeleteAsync(pageRole)).Returns(ValueTask.CompletedTask);

        pageRoleEventProcessingServiceMock
            .Setup(x => x.RaisePageRoleDeleteEventAsync(pageRole))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(pageRole);

        // Then
        pageRoleProcessingServiceMock.Verify(x => x.DeleteAsync(pageRole), Times.Once);
        pageRoleEventProcessingServiceMock.Verify(x => x.RaisePageRoleDeleteEventAsync(pageRole), Times.Once);
    }

}


















