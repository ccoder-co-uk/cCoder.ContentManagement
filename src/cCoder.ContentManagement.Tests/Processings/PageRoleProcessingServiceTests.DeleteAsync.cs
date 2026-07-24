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
using Moq;
using Xunit;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;
using LocalRole = cCoder.Data.Models.Security.Role;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationDeleteWhenUserCanDeletePageRoleForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);
        User user = TestUsers.WithPrivilege("pagerole_delete", 1);
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
                    RoleId = user.Roles.First().RoleId,
                    Role = user.Roles.First().Role,
                },
            ],
        };
        LocalPageRole link = new()
        {
            PageId = page.Id,
            RoleId = Guid.NewGuid(),
            Role = new LocalRole
            {
                Id = Guid.NewGuid(),
                AppId = 1,
                Name = "Editors",
                Privs = "page_read",
            },
        };
        currentUser = user;
        pageServiceMock.Setup(x => x.GetAllPage(true)).Returns(new[] { page }.AsQueryable());
        pageRoleServiceMock.Setup(x => x.GetAllPageRole(true)).Returns(new[] { link }.AsQueryable());
        pageRoleServiceMock.Setup(x => x.DeletePageRoleAsync(link)).Returns(ValueTask.CompletedTask);

        // When
        await pageRoleProcessingService.DeletePageRoleAsync(
        new LocalPageRole { PageId = link.PageId, RoleId = link.RoleId }
    );

        // Then
        pageServiceMock.Verify(x => x.GetAllPage(true), Times.Once);
        pageRoleServiceMock.Verify(x => x.GetAllPageRole(true), Times.Once);
        pageRoleServiceMock.Verify(
            x =>
                x.DeletePageRoleAsync(
                    It.Is<LocalPageRole>(item =>
                        item.RoleId == link.RoleId && item.PageId == link.PageId
                    )
                ),
            Times.Once
        );
    }

}






















