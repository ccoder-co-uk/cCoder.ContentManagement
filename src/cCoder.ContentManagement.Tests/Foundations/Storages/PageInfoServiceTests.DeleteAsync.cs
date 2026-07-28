// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
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

        pageInfoBrokerMock.Setup(expression: x => x.GetAllPageInfo(ignoreFilters: false))
            .Returns(value: new[] { ToDataPageInfo(pageInfo: pageInfo) }.AsQueryable());

        pageBrokerMock.Setup(expression: x => x.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_delete"));

        pageInfoBrokerMock.Setup(expression: x => x.DeletePageInfoAsync(deletedPageInfo: It.Is<DataPageInfo>(match: candidate => candidate.Id == pageInfo.Id)))
            .ReturnsAsync(value: 1);

        // When
        await pageInfoService.DeleteAsync(pageInfoId: 9);

        // Then
        pageInfoBrokerMock.Verify(expression: x => x.GetAllPageInfo(ignoreFilters: false), times: Times.Once);
        pageInfoBrokerMock.Verify(expression: x => x.DeletePageInfoAsync(deletedPageInfo: It.Is<DataPageInfo>(match: candidate => candidate.Id == pageInfo.Id)), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPages(ignoreFilters: true), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 9);

        pageInfoBrokerMock.Setup(expression: x => x.GetAllPageInfo(ignoreFilters: false))
            .Returns(value: new[] { ToDataPageInfo(pageInfo: pageInfo) }.AsQueryable());

        pageBrokerMock.Setup(expression: x => x.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await pageInfoService.DeleteAsync(pageInfoId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageInfoBrokerMock.Verify(expression: x => x.GetAllPageInfo(ignoreFilters: false), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPages(ignoreFilters: true), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}