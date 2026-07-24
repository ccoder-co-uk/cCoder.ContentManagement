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

        pageBrokerMock
            .Setup(
                expression: broker =>
                    broker.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        roleBrokerMock
            .Setup(
                expression: broker =>
                    broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[] { role }.AsQueryable());

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

        pageBrokerMock.VerifyAll();
        roleBrokerMock.VerifyAll();
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

        pageBrokerMock.VerifyNoOtherCalls();
        roleBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowDependencyExceptionWhenRoleBrokerFails()
    {
        // Given
        InvalidOperationException dependencyException = new(
            message: "Role query failed.");

        roleBrokerMock
            .Setup(
                expression: broker =>
                    broker.GetAllRoles(ignoreFilters: true))
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

        roleBrokerMock.VerifyAll();
        pageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowServiceExceptionWhenRoleBrokerFailsUnexpectedly()
    {
        // Given
        Exception serviceException = new(
            message: "Unexpected role query failure.");

        roleBrokerMock
            .Setup(
                expression: broker =>
                    broker.GetAllRoles(ignoreFilters: true))
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

        roleBrokerMock.VerifyAll();
        pageBrokerMock.VerifyNoOtherCalls();
    }
}