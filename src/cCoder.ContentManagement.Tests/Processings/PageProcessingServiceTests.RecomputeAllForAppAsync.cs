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
    public async Task ShouldRecomputePathsAndSaveWhenUserIsAppAdminForRecomputeAllForAppAsync()
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

        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        Page page = CreateRandomPage(user: actor);
        page.Name = "Home";
        page.Path = "home";

        page.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        pageServiceMock
            .Setup(expression: x => x.UpdatePageAsync(updatedPage: It.Is<Page>(match: updated => updated.Id == page.Id && updated.Path == string.Empty)))
            .Callback<Page>(action: updated => page.Path = updated.Path)
            .ReturnsAsync(value: page);

        // When
        await pageProcessingService.RecomputeAllForAppAsync(appId: 1);

        // Then
        page.Path.Should()
            .Be(expected: string.Empty);

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.UpdatePageAsync(updatedPage: It.Is<Page>(match: updated => updated.Id == page.Id && updated.Path == string.Empty)), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserIsNotAppAdminForRecomputeAllForAppAsync()
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

        User actor = TestUsers.WithoutPrivileges();

        currentUser = actor;

        // When
        Func<Task> act = async () => await pageProcessingService.RecomputeAllForAppAsync(appId: 1);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageServiceMock.VerifyNoOtherCalls();
    }

}