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

public partial class LayoutServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Layout layout = CreateRandomLayout(id: 9, appId: 7);

        layoutBrokerMock.Setup(expression: x => x.GetAllLayouts(ignoreFilters: false))
            .Returns(value: new[] { layout }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Layout_delete"));

        layoutBrokerMock.Setup(expression: x => x.DeleteLayoutAsync(deletedLayout: It.IsAny<CmsDataModels.Layout>()))
            .ReturnsAsync(value: 1);

        // When
        await layoutService.DeleteAsync(layoutId: 9);

        // Then
        layoutBrokerMock.Verify(expression: x => x.GetAllLayouts(ignoreFilters: false), times: Times.Once);
        layoutBrokerMock.Verify(expression: x => x.DeleteLayoutAsync(deletedLayout: It.Is<CmsDataModels.Layout>(match: actual => actual.Id == layout.Id)), times: Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Layout_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Layout layout = CreateRandomLayout(id: 9, appId: 7);

        layoutBrokerMock.Setup(expression: x => x.GetAllLayouts(ignoreFilters: false))
            .Returns(value: new[] { layout }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Layout_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await layoutService.DeleteAsync(layoutId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        layoutBrokerMock.Verify(expression: x => x.GetAllLayouts(ignoreFilters: false), times: Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Layout_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}