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
using DataPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();
        IQueryable<DataPageInfo> pageInfos = new[] { ToDataPageInfo(pageInfo) }.AsQueryable();

        pageInfoBrokerMock.Setup(x => x.GetAllPageInfo(false)).Returns(pageInfos);

        // When
        IQueryable<PageInfo> result = pageInfoService.GetAll();

        // Then
        result.Should().BeEquivalentTo([pageInfo]);
        pageInfoBrokerMock.Verify(x => x.GetAllPageInfo(false), Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}




















