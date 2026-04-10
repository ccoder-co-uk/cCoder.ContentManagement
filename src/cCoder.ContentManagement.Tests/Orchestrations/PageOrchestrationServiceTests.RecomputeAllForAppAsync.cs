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

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenRecomputeAllForAppAsync()
    {
        pageProcessingServiceMock
            .Setup(x => x.RecomputeAllForAppAsync(1))
            .Returns(ValueTask.CompletedTask);

        await orchestrationService.RecomputeAllForAppAsync(1);

        pageProcessingServiceMock.Verify(x => x.RecomputeAllForAppAsync(1), Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}















