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

public partial class PackageItemOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        PackageItem[] entities = [CreateRandomPackageItem()];
        packageItemProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        packageItemProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        packageItemProcessingServiceMock.VerifyNoOtherCalls();
        packageItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}



















