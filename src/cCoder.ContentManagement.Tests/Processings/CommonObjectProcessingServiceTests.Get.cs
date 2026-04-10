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



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject();
        commonObjectServiceMock.Setup(x => x.Get(commonObject.Id)).Returns(commonObject);

        // When
        CommonObject result = commonObjectProcessingService.Get(
            commonObject.Id
        );

        // Then
        result.Should().BeSameAs(commonObject);
        commonObjectServiceMock.Verify(x => x.Get(commonObject.Id), Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}















