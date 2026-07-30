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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        DataPageInfo submitted = null;

        pageBrokerMock.Setup(expression: x => x.GetAllPagesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_update"));

        pageInfoBrokerMock
            .Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: It.IsAny<DataPageInfo>()))
            .Callback<DataPageInfo>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (DataPageInfo value) => value);

        // When
        PageInfo result = await pageInfoService.UpdatePageInfoAsync(updatedPageInfo: pageInfo);

        // Then

        result.Should()
            .BeSameAs(expected: pageInfo);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: pageInfo);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: pageInfo);

        result.Should()
            .BeEquivalentTo(expectation: pageInfo);

        pageInfoBrokerMock.Verify(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: It.IsAny<DataPageInfo>()), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPagesIgnoringFilters(), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_update"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        pageBrokerMock.Setup(expression: x => x.GetAllPagesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.Page { Id = pageInfo.PageId, AppId = 7 } }.AsQueryable());

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await pageInfoService.UpdatePageInfoAsync(updatedPageInfo: pageInfo);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageInfoBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPagesIgnoringFilters(), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "PageInfo_update"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}