// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleImportPersistenceProcessingServiceTests
{
    [Fact]
    public async Task ShouldSynchronizePageRolesAsync()
    {
        // Given
        PageRole retainedPageRole = CreatePageRole(
            pageId: 1,
            roleId: Guid.NewGuid());

        PageRole deletedPageRole = CreatePageRole(
            pageId: 1,
            roleId: Guid.NewGuid());

        PageRole addedPageRole = CreatePageRole(
            pageId: 2,
            roleId: Guid.NewGuid());

        PageRole[] incomingPageRoles =
        [
            retainedPageRole,
            addedPageRole
        ];

        pageRoleBrokerMock
            .Setup(
                expression: broker =>
                    broker.GetAllPageRoles(ignoreFilters: true))
            .Returns(
                value: new[]
                {
                    retainedPageRole,
                    deletedPageRole
                }.AsQueryable());

        pageRoleBrokerMock
            .Setup(
                expression: broker =>
                    broker.DeleteAllPageRolesAsync(
                        It.Is<PageRole[]>(
                            match: pageRoles =>
                                pageRoles.Length == 1
                                && pageRoles[0] == deletedPageRole)))
            .Returns(value: ValueTask.CompletedTask);

        pageRoleBrokerMock
            .Setup(
                expression: broker =>
                    broker.AddPageRoleAsync(
                        newPageRole: addedPageRole))
            .ReturnsAsync(value: addedPageRole);

        // When
        await processingService.SynchronizePageRolesAsync(
            pageRoles: incomingPageRoles);

        // Then
        pageRoleBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenPageRolesAreNullAsync()
    {
        // Given
        PageRole[] invalidPageRoles = null;

        // When
        Func<Task> action = async () =>
            await processingService.SynchronizePageRolesAsync(
                pageRoles: invalidPageRoles);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementValidationException>();

        pageRoleBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionWhenBrokerFailsAsync()
    {
        // Given
        PageRole[] pageRoles =
        [
            CreatePageRole(
                pageId: 1,
                roleId: Guid.NewGuid())
        ];

        InvalidOperationException dependencyException = new(
            message: "Page role query failed.");

        pageRoleBrokerMock
            .Setup(
                expression: broker =>
                    broker.GetAllPageRoles(ignoreFilters: true))
            .Throws(exception: dependencyException);

        // When
        Func<Task> action = async () =>
            await processingService.SynchronizePageRolesAsync(
                pageRoles: pageRoles);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementDependencyException>();

        pageRoleBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionWhenBrokerFailsUnexpectedlyAsync()
    {
        // Given
        PageRole[] pageRoles =
        [
            CreatePageRole(
                pageId: 1,
                roleId: Guid.NewGuid())
        ];

        Exception serviceException = new(
            message: "Unexpected page role query failure.");

        pageRoleBrokerMock
            .Setup(
                expression: broker =>
                    broker.GetAllPageRoles(ignoreFilters: true))
            .Throws(exception: serviceException);

        // When
        Func<Task> action = async () =>
            await processingService.SynchronizePageRolesAsync(
                pageRoles: pageRoles);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementServiceException>();

        pageRoleBrokerMock.VerifyAll();
    }
}