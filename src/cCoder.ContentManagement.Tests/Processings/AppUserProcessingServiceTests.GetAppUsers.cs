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

public partial class AppUserProcessingServiceTests
{
    [Fact]
    public void ShouldReturnRoleUsersWhenAppExistsForGetAppUsers()
    {

        // Given
        User appUser = TestUsers.WithPrivilege(privilege: "page_read", appId: 1);
        App app = CreateRandomApp();
        app.Id = 1;

        app.Roles =
        [
            new Role
            {
                Id = Guid.NewGuid(),
                AppId = app.Id,
                Name = "Users",
                Privs = "page_read",
                Users =
                [
                    new UserRole
                    {
                        User = appUser,
                        UserId = appUser.Id,
                        RoleId = appUser.Roles.First()
            .RoleId,
                    },
                ],
            },
        ];

        appServiceMock.Setup(expression: x => x.GetApp(appId: app.Id))
            .Returns(value: app);

        // When
        User[] result = appUserProcessingService.GetAppUsers(appId: app.Id)
            .ToArray();

        // Then
        result.Should()
            .ContainSingle();

        result[0].Should()
            .BeSameAs(expected: appUser);

        appServiceMock.Verify(expression: x => x.GetApp(appId: app.Id), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenAppDoesNotExistForGetAppUsers()
    {
        // Given
        appServiceMock.Setup(expression: x => x.GetApp(appId: 1))
            .Returns(value: (App)null!);

        // When
        Action act = () => appUserProcessingService.GetAppUsers(appId: 1)
            .ToArray();

        // Then
        act.Should()
            .Throw<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appServiceMock.Verify(expression: x => x.GetApp(appId: 1), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}