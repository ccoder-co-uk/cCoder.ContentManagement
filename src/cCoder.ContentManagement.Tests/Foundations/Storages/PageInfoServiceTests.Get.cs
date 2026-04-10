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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 7);

        pageInfoBrokerMock.Setup(x => x.GetAllPageInfo(false)).Returns(new[] { ToDataPageInfo(pageInfo) }.AsQueryable());

        // When
        PageInfo result = pageInfoService.Get(7);

        // Then
        result.Should().BeEquivalentTo(pageInfo);
        pageInfoBrokerMock.Verify(x => x.GetAllPageInfo(false), Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}




















