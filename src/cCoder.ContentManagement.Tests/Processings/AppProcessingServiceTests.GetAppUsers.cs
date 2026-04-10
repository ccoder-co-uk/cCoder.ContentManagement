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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public void ShouldReturnRoleUsersWhenAppExistsForGetAppUsers()
    {
        // Then
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User appUser = TestUsers.WithPrivilege("page_read", 1);
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
                        RoleId = appUser.Roles.First().RoleId,
                    },
                ],
            },
        ];
        appServiceMock.Setup(x => x.Get(app.Id)).Returns(app);

        // When
        User[] result = appProcessingService.GetAppUsers(app.Id).ToArray();

        // Then
        result.Should().ContainSingle();
        result[0].Should().BeSameAs(appUser);
        appServiceMock.Verify(x => x.Get(app.Id), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenAppDoesNotExistForGetAppUsers()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        appServiceMock.Setup(x => x.Get(1)).Returns((App)null!);

        // When
        Action act = () => appProcessingService.GetAppUsers(1).ToArray();

        // Then
        act.Should().Throw<SecurityException>().WithMessage("Access Denied!");
        appServiceMock.Verify(x => x.Get(1), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}
















