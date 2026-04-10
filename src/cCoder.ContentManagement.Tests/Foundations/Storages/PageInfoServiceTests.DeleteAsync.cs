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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;
using DataPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 9);

        pageInfoBrokerMock.Setup(x => x.GetAllPageInfo(false)).Returns(new[] { ToDataPageInfo(pageInfo) }.AsQueryable());

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "PageInfo_delete"));
        pageInfoBrokerMock.Setup(x => x.DeletePageInfoAsync(It.Is<DataPageInfo>(candidate => candidate.Id == pageInfo.Id))).ReturnsAsync(1);

        // When
        await pageInfoService.DeleteAsync(9);

        // Then
        pageInfoBrokerMock.Verify(x => x.GetAllPageInfo(false), Times.Once);
        pageInfoBrokerMock.Verify(x => x.DeletePageInfoAsync(It.Is<DataPageInfo>(candidate => candidate.Id == pageInfo.Id)), Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PageInfo_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 9);

        pageInfoBrokerMock.Setup(x => x.GetAllPageInfo(false)).Returns(new[] { ToDataPageInfo(pageInfo) }.AsQueryable());

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "PageInfo_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await pageInfoService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageInfoBrokerMock.Verify(x => x.GetAllPageInfo(false), Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PageInfo_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}
















