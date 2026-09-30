// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;
using LocalRole = cCoder.Data.Models.Security.Role;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    [Fact]
    public async Task DeletePageRole_WhenPageRoleExists_ShouldPersistDeleteAsync()
    {
        // Given
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

        pageRoleServiceMock.Setup(expression: x => x.GetAllPageRoles(ignoreFilters: true))
            .Returns(value: new[] { link }.AsQueryable());

        pageRoleServiceMock.Setup(expression: x => x.DeletePageRoleAsync(deletedPageRole: link))
            .Returns(value: ValueTask.CompletedTask);

        // When

        await pageRoleProcessingService.DeletePageRoleAsync(
deletedPageRole: new LocalPageRole { PageId = link.PageId, RoleId = link.RoleId }
    );

        // Then
        pageRoleServiceMock.Verify(expression: x => x.GetAllPageRoles(ignoreFilters: true), times: Times.Once);

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