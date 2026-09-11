// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleImportLookupProcessingServiceTests
{
    [Fact]
    public void ShouldResolvePageRole()
    {
        // Given
        const int appId = 42;
        const string path = "Students";
        const string roleName = "Teachers";
        Page page = CreatePage(appId: appId, path: path);
        Role role = CreateRole(appId: appId, roleName: roleName);

        pageRoleServiceMock.Setup(expression: service => service.ResolvePageRole(
                appId: appId,
                path: path,
                roleName: roleName))
            .Returns(value: new PageRole { PageId = page.Id, RoleId = role.Id });

        // When
        PageRole result = processingService.ResolvePageRole(
            appId: appId,
            path: path,
            roleName: roleName);

        // Then
        result.PageId.Should()
            .Be(expected: page.Id);

        result.RoleId.Should()
            .Be(expected: role.Id);

        pageRoleServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldResolveRootPageRoleWhenPathIsEmpty()
    {
        // Given
        const int appId = 42;
        const string path = "";
        const string roleName = "Guests";
        Page page = CreatePage(appId: appId, path: path);
        Role role = CreateRole(appId: appId, roleName: roleName);

        pageRoleServiceMock.Setup(expression: service => service.ResolvePageRole(
                appId: appId,
                path: path,
                roleName: roleName))
            .Returns(value: new PageRole { PageId = page.Id, RoleId = role.Id });

        // When
        PageRole result = processingService.ResolvePageRole(
            appId: appId,
            path: path,
            roleName: roleName);

        // Then
        result.PageId.Should()
            .Be(expected: page.Id);

        result.RoleId.Should()
            .Be(expected: role.Id);

        pageRoleServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenAppIdIsInvalid()
    {
        // Given
        const int invalidAppId = 0;

        // When
        Action action = () =>
            processingService.ResolvePageRole(
                appId: invalidAppId,
                path: "Students",
                roleName: "Teachers");

        // Then
        action.Should()
            .Throw<ContentManagementValidationException>();

        pageRoleServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowDependencyExceptionWhenRoleBrokerFails()
    {
        // Given
        InvalidOperationException dependencyException = new(
            message: "Role query failed.");

        pageRoleServiceMock
            .Setup(expression: service => service.ResolvePageRole(
                appId: It.IsAny<int>(),
                path: It.IsAny<string>(),
                roleName: It.IsAny<string>()))
            .Throws(exception: dependencyException);

        // When
        Action action = () =>
            processingService.ResolvePageRole(
                appId: 42,
                path: "Students",
                roleName: "Teachers");

        // Then
        action.Should()
            .Throw<ContentManagementDependencyException>();

        pageRoleServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowServiceExceptionWhenRoleBrokerFailsUnexpectedly()
    {
        // Given
        Exception serviceException = new(
            message: "Unexpected role query failure.");

        pageRoleServiceMock
            .Setup(expression: service => service.ResolvePageRole(
                appId: It.IsAny<int>(),
                path: It.IsAny<string>(),
                roleName: It.IsAny<string>()))
            .Throws(exception: serviceException);

        // When
        Action action = () =>
            processingService.ResolvePageRole(
                appId: 42,
                path: "Students",
                roleName: "Teachers");

        // Then
        action.Should()
            .Throw<ContentManagementServiceException>();

        pageRoleServiceMock.VerifyAll();
    }
}