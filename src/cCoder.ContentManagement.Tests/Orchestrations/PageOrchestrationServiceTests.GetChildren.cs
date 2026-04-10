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

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetChildren()
    {
        Page[] expected = [CreateRandomPage()];
        pageProcessingServiceMock.Setup(x => x.GetChildren(1)).Returns(expected);

        var result = orchestrationService.GetChildren(1).ToArray();

        result.Select(item => item.Id).Should().Equal(expected.Select(item => item.Id));
        pageProcessingServiceMock.Verify(x => x.GetChildren(1), Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















