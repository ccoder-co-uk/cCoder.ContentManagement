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



using Moq;
using Xunit;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;
using SecurityDataModels = cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseDataContextWhenUserCanCreatePageRoleForAddAsync()
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

        User user = TestUsers.WithPrivilege("pagerole_create", 1);
        UserRole currentUserRole = user.Roles.First();
        SecurityDataModels.Role roleToAdd = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "page_read",
        };
        Page page = new()
        {
            Id = 8,
            AppId = 1,
            Name = "Home",
            Path = string.Empty,
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Home" }],
            Roles =
            [
                new LocalPageRole
                {
                    PageId = 8,
                    RoleId = currentUserRole.RoleId,
                    Role = currentUserRole.Role,
                }
            ],
        };
        LocalPageRole link = new() { PageId = page.Id, RoleId = roleToAdd.Id };
        currentUser = user;
        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[] { roleToAdd }.AsQueryable());
        pageServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { page }.AsQueryable());
        pageRoleServiceMock.Setup(x => x.AddAsync(link)).ReturnsAsync(link);

        // When
        LocalPageRole result = await pageRoleProcessingService.AddAsync(link);

        // Then
        Assert.Same(link, result);
        pageRoleServiceMock.Verify(x => x.AddAsync(link), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
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

        SecurityDataModels.Role roleToAdd = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "page_read",
        };
        Page page = new()
        {
            Id = 8,
            AppId = 1,
            Name = "Home",
            Path = string.Empty,
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Home" }],
            Roles = [],
        };
        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[] { roleToAdd }.AsQueryable());
        pageServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { page }.AsQueryable());

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await pageRoleProcessingService.AddAsync(
                new LocalPageRole { PageId = page.Id, RoleId = roleToAdd.Id }
            )
        );

        // Then
    }

}




















