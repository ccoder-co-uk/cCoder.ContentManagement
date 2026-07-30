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

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUserCanDeletePageForDeleteAsync()
    {
        // Given
        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationManagerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationManagerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User user = TestUsers.WithPrivilege(privilege: "page_delete", appId: 1);
        Page page = CreateRandomPage(user: user);
        currentUser = user;

        pageServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { page }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.DeleteAsync(pageId: page.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await pageProcessingService.DeleteAsync(pageId: page.Id);

        // Then
        pageServiceMock.Verify(expression: x => x.GetAllPage(), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.DeleteAsync(pageId: page.Id), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        authorizationManagerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationManagerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationManagerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        Page page = CreateRandomPage();
        currentUser = TestUsers.WithoutPrivileges();

        pageServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { page }.AsQueryable());

        // When
        Func<Task> act = async () => await pageProcessingService.DeleteAsync(pageId: page.Id);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>();

        pageServiceMock.Verify(expression: x => x.GetAllPage(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}