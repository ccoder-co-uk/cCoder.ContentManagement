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

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Layout[] entities = [CreateRandomLayout()];
        layoutProcessingServiceMock.Setup(x => x.Get(entities[0].Id)).Returns(entities[0]);
        layoutEventProcessingServiceMock.Setup(x => x.RaiseLayoutDeleteEventAsync(entities[0])).Returns(ValueTask.CompletedTask);
        layoutProcessingServiceMock.Setup(x => x.DeleteAsync(entities[0].Id)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        layoutProcessingServiceMock.Verify(x => x.Get(entities[0].Id), Times.Once);
        layoutEventProcessingServiceMock.Verify(x => x.RaiseLayoutDeleteEventAsync(entities[0]), Times.Once);
        layoutProcessingServiceMock.Verify(x => x.DeleteAsync(entities[0].Id), Times.Once);
    }

}




















