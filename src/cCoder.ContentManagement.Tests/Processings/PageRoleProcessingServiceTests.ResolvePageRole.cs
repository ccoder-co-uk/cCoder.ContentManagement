// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    [Fact]
    public void ResolvePageRole_WhenGivenImportNames_ShouldResolveThroughFoundation()
    {
        // Given
        const int appId = 42;
        const string path = "Students";
        const string roleName = "Teachers";

        PageRole resolvedPageRole = new()
        {
            PageId = 123,
            RoleId = Guid.NewGuid()
        };

        pageRoleServiceMock
            .Setup(expression: service =>
                service.ResolvePageRoleByNames(
                    pageRole: It.Is<PageRole>(match: match =>
                        match.Page.AppId == appId
                        && match.Page.Path == path
                        && match.Role.AppId == appId
                        && match.Role.Name == roleName)))
            .Returns(value: resolvedPageRole);

        // When
        PageRole result = pageRoleProcessingService.ResolvePageRole(
            appId: appId,
            path: path,
            roleName: roleName);

        // Then
        result.Should()
            .BeSameAs(expected: resolvedPageRole);

        pageRoleServiceMock.VerifyAll();
    }

    [Fact]
    public void ResolvePageRole_WhenGivenKeys_ShouldResolveThroughFoundation()
    {
        // Given
        PageRole pageRole = new()
        {
            PageId = 123,
            RoleId = Guid.NewGuid()
        };

        PageRole resolvedPageRole = new()
        {
            PageId = pageRole.PageId,
            RoleId = pageRole.RoleId,
            Page = new Page { Id = pageRole.PageId },
            Role = new Role { Id = pageRole.RoleId }
        };

        pageRoleServiceMock
            .Setup(expression: service =>
                service.ResolvePageRoleByIds(pageRole: pageRole))
            .Returns(value: resolvedPageRole);

        // When
        PageRole result = pageRoleProcessingService.ResolvePageRole(
            pageRole: pageRole);

        // Then
        result.Should()
            .BeSameAs(expected: resolvedPageRole);

        pageRoleServiceMock.VerifyAll();
    }
}