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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        Guid id = Guid.NewGuid();
        PackageItem entity = CreateRandomPackageItem();
        packageItemProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        packageItemProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);
        packageItemEventProcessingServiceMock
            .Setup(x => x.RaisePackageItemDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        await orchestrationService.DeleteAsync(id);

        packageItemProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        packageItemProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        packageItemProcessingServiceMock.VerifyNoOtherCalls();
        packageItemEventProcessingServiceMock.Verify(x => x.RaisePackageItemDeleteEventAsync(entity), Times.Once);
        packageItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}



















