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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 0);

        DataPageInfo submitted = null;

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "PageInfo_create"));

        pageInfoBrokerMock
            .Setup(x =>
                x.AddPageInfoAsync(
                    It.IsAny<DataPageInfo>()
                )
            )
            .Callback<DataPageInfo>(candidate => submitted = candidate)
            .ReturnsAsync((DataPageInfo value) => value);

        // When
        PageInfo result = await pageInfoService.AddAsync(pageInfo);

        // Then
        result.Should().BeSameAs(pageInfo);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(pageInfo);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(pageInfo, options => options.Excluding(candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(pageInfo, options => options.Excluding(candidate => candidate.Id));

        pageInfoBrokerMock.Verify(
            x =>
                x.AddPageInfoAsync(
                    It.Is<DataPageInfo>(candidate => candidate.Id == pageInfo.Id)
                ),
            Times.Once
        );
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PageInfo_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 0);

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "PageInfo_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await pageInfoService.AddAsync(pageInfo);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "PageInfo_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















