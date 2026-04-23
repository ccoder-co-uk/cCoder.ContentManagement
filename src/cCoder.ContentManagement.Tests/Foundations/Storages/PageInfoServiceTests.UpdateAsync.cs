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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        DataPageInfo submitted = null;

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "PageInfo_update"));

        pageInfoBrokerMock
            .Setup(x => x.UpdatePageInfoAsync(It.IsAny<DataPageInfo>()))
            .Callback<DataPageInfo>(candidate => submitted = candidate)
            .ReturnsAsync((DataPageInfo value) => value);

        // When
        PageInfo result = await pageInfoService.UpdateAsync(pageInfo);

        // Then
        result.Should().BeSameAs(pageInfo);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(pageInfo);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(pageInfo);
        result.Should().BeEquivalentTo(pageInfo);
        pageInfoBrokerMock.Verify(x => x.UpdatePageInfoAsync(It.IsAny<DataPageInfo>()), Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PageInfo_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "PageInfo_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await pageInfoService.UpdateAsync(pageInfo);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PageInfo_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















