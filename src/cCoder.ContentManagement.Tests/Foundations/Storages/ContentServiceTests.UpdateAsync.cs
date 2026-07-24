// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 7);

        CmsDataModels.Content submitted = null;

        pageBrokerMock.Setup(expression: x => x.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_update"));

        contentBrokerMock
            .Setup(expression: x => x.UpdateContentAsync(updatedContent: It.IsAny<CmsDataModels.Content>()))
            .Callback<CmsDataModels.Content>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Content value) => value);

        // When
        Content result = await contentService.UpdateContentAsync(updatedContent: content);

        // Then

        result.Should()
            .BeSameAs(expected: content);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: content);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: content);

        result.Should()
            .BeEquivalentTo(expectation: content);

        contentBrokerMock.Verify(expression: x => x.UpdateContentAsync(updatedContent: It.IsAny<CmsDataModels.Content>()), times: Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPages(ignoreFilters: true), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 7);

        pageBrokerMock.Setup(expression: x => x.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await contentService.UpdateContentAsync(updatedContent: content);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(expression: x => x.GetAllPages(ignoreFilters: true), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Content_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}