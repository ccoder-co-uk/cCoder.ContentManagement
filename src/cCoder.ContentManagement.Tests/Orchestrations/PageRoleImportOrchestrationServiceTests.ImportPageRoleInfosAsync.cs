// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRoleImportOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldImportDistinctResolvedPageRoleInfosAsync()
    {
        // Given
        const int appId = 42;

        PageRoleInfo pageRoleInfo = CreatePageRoleInfo(
            path: "Students",
            roleName: "Teachers");

        PageRole resolvedPageRole = CreatePageRole(
            pageId: 123,
            roleId: new Guid(
                g: "5fe70497-0cb4-4e1f-8f72-f0370b2af448"));

        PageRoleInfo[] pageRoleInfos =
        [
            pageRoleInfo,
            pageRoleInfo
        ];

        pageRoleProcessingServiceMock
            .Setup(expression: service =>
                service.ResolvePageRole(
                    appId: appId,
                    path: pageRoleInfo.Path,
                    roleName: pageRoleInfo.Role))
            .Returns(value: resolvedPageRole);

        persistenceProcessingServiceMock
            .Setup(
                expression: service =>
                    service.SynchronizePageRolesAsync(
                        pageRoles: It.Is<PageRole[]>(
                            match: pageRoles =>
                                pageRoles.Length == 1
                                && pageRoles[0].PageId
                                    == resolvedPageRole.PageId
                                && pageRoles[0].RoleId
                                    == resolvedPageRole.RoleId)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.ImportPageRoleInfosAsync(
            appId: appId,
            pageRoleInfos: pageRoleInfos);

        // Then
        pageRoleProcessingServiceMock.Verify(
            expression: service =>
                service.ResolvePageRole(
                    appId: appId,
                    path: pageRoleInfo.Path,
                    roleName: pageRoleInfo.Role),
            times: Times.Exactly(callCount: 2));

        persistenceProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldFailWhenPageRoleCannotBeResolvedAsync()
    {
        // Given
        const int appId = 42;

        PageRoleInfo pageRoleInfo = CreatePageRoleInfo(
            path: string.Empty,
            roleName: "Guests");

        PageRole unresolvedPageRole = CreatePageRole(
            pageId: 123,
            roleId: Guid.Empty);

        pageRoleProcessingServiceMock
            .Setup(expression: service =>
                service.ResolvePageRole(
                    appId: appId,
                    path: pageRoleInfo.Path,
                    roleName: pageRoleInfo.Role))
            .Returns(value: unresolvedPageRole);

        // When
        Func<Task> action = async () =>
            await orchestrationService.ImportPageRoleInfosAsync(
                appId: appId,
                pageRoleInfos: [pageRoleInfo]);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementValidationException>();

        pageRoleProcessingServiceMock.VerifyAll();

        persistenceProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenPageRoleInfosAreNullAsync()
    {
        // Given
        PageRoleInfo[] invalidPageRoleInfos = null;

        // When
        Func<Task> action = async () =>
            await orchestrationService.ImportPageRoleInfosAsync(
                appId: 42,
                pageRoleInfos: invalidPageRoleInfos);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementValidationException>();

        pageRoleProcessingServiceMock.VerifyNoOtherCalls();
        persistenceProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionWhenLookupFailsAsync()
    {
        // Given
        PageRoleInfo pageRoleInfo = CreatePageRoleInfo(
            path: "Students",
            roleName: "Teachers");

        ContentManagementDependencyException dependencyException = new(
            innerException: new InvalidOperationException(
                message: "Lookup failed."));

        pageRoleProcessingServiceMock
            .Setup(expression: service =>
                service.ResolvePageRole(
                    appId: 42,
                    path: pageRoleInfo.Path,
                    roleName: pageRoleInfo.Role))
            .Throws(exception: dependencyException);

        // When
        Func<Task> action = async () =>
            await orchestrationService.ImportPageRoleInfosAsync(
                appId: 42,
                pageRoleInfos: [pageRoleInfo]);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementDependencyException>();

        pageRoleProcessingServiceMock.VerifyAll();
        persistenceProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionWhenLookupFailsUnexpectedlyAsync()
    {
        // Given
        PageRoleInfo pageRoleInfo = CreatePageRoleInfo(
            path: "Students",
            roleName: "Teachers");

        Exception serviceException = new(
            message: "Unexpected lookup failure.");

        pageRoleProcessingServiceMock
            .Setup(expression: service =>
                service.ResolvePageRole(
                    appId: 42,
                    path: pageRoleInfo.Path,
                    roleName: pageRoleInfo.Role))
            .Throws(exception: serviceException);

        // When
        Func<Task> action = async () =>
            await orchestrationService.ImportPageRoleInfosAsync(
                appId: 42,
                pageRoleInfos: [pageRoleInfo]);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementServiceException>();

        pageRoleProcessingServiceMock.VerifyAll();
        persistenceProcessingServiceMock.VerifyNoOtherCalls();
    }
}