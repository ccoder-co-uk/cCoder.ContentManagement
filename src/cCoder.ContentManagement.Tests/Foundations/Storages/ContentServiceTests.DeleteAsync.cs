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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 9);

        contentBrokerMock.Setup(expression: x => x.GetAllContents())
            .Returns(value: new[] { content }.AsQueryable());

        pageBrokerMock.Setup(expression: x => x.GetAllPagesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());

        authorizationManagerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_delete"));

        contentBrokerMock
            .Setup(
expression: x =>
                    x.DeleteContentAsync(
deletedContent: It.Is<CmsDataModels.Content>(match: item => item.Id == content.Id)
                    )
            )
            .ReturnsAsync(value: 1);

        // When
        await contentService.DeleteAsync(contentId: 9);

        // Then
        contentBrokerMock.Verify(expression: x => x.GetAllContents(), times: Times.Once);

        contentBrokerMock.Verify(
expression: x => x.DeleteContentAsync(deletedContent: It.Is<CmsDataModels.Content>(match: item => item.Id == content.Id)),
times: Times.Once
        );

        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPagesIgnoringFilters(), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 9);

        contentBrokerMock.Setup(expression: x => x.GetAllContents())
            .Returns(value: new[] { content }.AsQueryable());

        pageBrokerMock.Setup(expression: x => x.GetAllPagesIgnoringFilters())
            .Returns(value: new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());

        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await contentService.DeleteAsync(contentId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        contentBrokerMock.Verify(expression: x => x.GetAllContents(), times: Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPagesIgnoringFilters(), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_delete"), times: Times.Once);
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}