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
        User user = TestUsers.WithPrivilege(privilege: "pagerole_create", appId: 1);
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
        pageRoleServiceMock.Setup(expression: x => x.UserCanAddPageRole(link))
            .Returns(value: true);

        pageRoleServiceMock.Setup(expression: x => x.PageRoleExists(link))
            .Returns(value: false);

        pageRoleServiceMock.Setup(expression: x => x.AddPageRoleAsync(newPageRole: link))
            .ReturnsAsync(value: link);

        // When
        LocalPageRole result = await pageRoleProcessingService.AddPageRoleAsync(newPageRole: link);

        // Then
        Assert.Same(expected: link, actual: result);
        pageRoleServiceMock.Verify(expression: x => x.AddPageRoleAsync(newPageRole: link), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
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

        pageRoleServiceMock.Setup(expression: x => x.UserCanAddPageRole(
                It.IsAny<LocalPageRole>()))
            .Returns(value: false);

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await pageRoleProcessingService.AddPageRoleAsync(
newPageRole: new LocalPageRole { PageId = page.Id, RoleId = roleToAdd.Id }
            )
        );

        // Then
    }

}