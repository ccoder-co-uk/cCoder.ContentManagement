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
        authorizationManagerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User user = TestUsers.WithPrivilege(privilege: "pagerole_delete", appId: 1);

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
                    RoleId = user.Roles.First()
            .RoleId,
                    Role = user.Roles.First()
            .Role,
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

        pageBrokerMock.Setup(expression: x => x.GetAllPagesIgnoringFilters())
            .Returns(value: new[] { page }.AsQueryable());

        pageRoleServiceMock.Setup(expression: x => x.GetAllPageRole(ignoreFilters: true))
            .Returns(value: new[] { link }.AsQueryable());

        pageRoleServiceMock.Setup(expression: x => x.DeletePageRoleAsync(deletedPageRole: link))
            .Returns(value: ValueTask.CompletedTask);

        // When

        await pageRoleProcessingService.DeletePageRoleAsync(
deletedPageRole: new LocalPageRole { PageId = link.PageId, RoleId = link.RoleId }
    );

        // Then
        pageBrokerMock.Verify(expression: x => x.GetAllPagesIgnoringFilters(), times: Times.Once);
        pageRoleServiceMock.Verify(expression: x => x.GetAllPageRole(ignoreFilters: true), times: Times.Once);

        pageRoleServiceMock.Verify(
expression: x =>
                x.DeletePageRoleAsync(
deletedPageRole: It.Is<LocalPageRole>(match: item =>
                        item.RoleId == link.RoleId && item.PageId == link.PageId
                    )
                ),
times: Times.Once
        );
    }

}