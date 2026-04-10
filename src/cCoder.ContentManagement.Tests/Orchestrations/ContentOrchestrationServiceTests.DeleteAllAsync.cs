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

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Content[] entities = [CreateRandomContent()];
        contentProcessingServiceMock.Setup(x => x.Get(entities[0].Id)).Returns(entities[0]);
        contentEventProcessingServiceMock.Setup(x => x.RaiseContentDeleteEventAsync(entities[0])).Returns(ValueTask.CompletedTask);
        contentProcessingServiceMock.Setup(x => x.DeleteAsync(entities[0].Id)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        contentProcessingServiceMock.Verify(x => x.Get(entities[0].Id), Times.Once);
        contentEventProcessingServiceMock.Verify(x => x.RaiseContentDeleteEventAsync(entities[0]), Times.Once);
        contentProcessingServiceMock.Verify(x => x.DeleteAsync(entities[0].Id), Times.Once);
    }

}




















