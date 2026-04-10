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

public partial class PackageItemOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        PackageItem entity = CreateRandomPackageItem();
        packageItemProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);
        packageItemEventProcessingServiceMock
            .Setup(x => x.RaisePackageItemUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        PackageItem result = await orchestrationService.UpdateAsync(entity);

        result.Should().BeSameAs(entity);
        packageItemProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        packageItemProcessingServiceMock.VerifyNoOtherCalls();
        packageItemEventProcessingServiceMock.Verify(x => x.RaisePackageItemUpdateEventAsync(entity), Times.Once);
        packageItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}



















