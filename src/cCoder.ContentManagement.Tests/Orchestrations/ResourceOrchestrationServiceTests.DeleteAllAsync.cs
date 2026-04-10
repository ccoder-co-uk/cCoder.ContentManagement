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

public partial class ResourceOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Resource[] entities = [CreateRandomResource()];
        resourceProcessingServiceMock.Setup(x => x.Get(entities[0].Id)).Returns(entities[0]);
        resourceEventProcessingServiceMock.Setup(x => x.RaiseResourceDeleteEventAsync(entities[0])).Returns(ValueTask.CompletedTask);
        resourceProcessingServiceMock.Setup(x => x.DeleteAsync(entities[0].Id)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        resourceProcessingServiceMock.Verify(x => x.Get(entities[0].Id), Times.Once);
        resourceEventProcessingServiceMock.Verify(x => x.RaiseResourceDeleteEventAsync(entities[0]), Times.Once);
        resourceProcessingServiceMock.Verify(x => x.DeleteAsync(entities[0].Id), Times.Once);
    }

}




















