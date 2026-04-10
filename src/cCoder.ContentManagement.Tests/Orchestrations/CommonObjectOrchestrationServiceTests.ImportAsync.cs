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

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnProcessingResultsWhenImportAsync()
    {
        CommonObject[] items = [CreateRandomCommonObject()];
        cCoder.ContentManagement.Models.Result<CommonObject>[] expectedResults = [];
        commonObjectProcessingServiceMock.Setup(x => x.ImportAsync(items)).ReturnsAsync(expectedResults);

        IEnumerable<cCoder.ContentManagement.Models.Result<CommonObject>> result = await orchestrationService.ImportAsync(items);

        result.Should().BeSameAs(expectedResults);
        commonObjectProcessingServiceMock.Verify(x => x.ImportAsync(items), Times.Once);
        commonObjectProcessingServiceMock.VerifyNoOtherCalls();
        commonObjectEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}
























