// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.Security;
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
            roleId: Guid.NewGuid());

        PageRoleInfo[] pageRoleInfos =
        [
            pageRoleInfo,
            pageRoleInfo
        ];

        lookupProcessingServiceMock
            .Setup(
                expression: service =>
                    service.ResolvePageRole(
                        appId: appId,
                        path: pageRoleInfo.Path,
                        roleName: pageRoleInfo.Role))
            .Returns(value: resolvedPageRole);

        persistenceProcessingServiceMock
            .Setup(
                expression: service =>
                    service.SynchronizePageRolesAsync(
                        It.Is<PageRole[]>(
                            match: pageRoles =>
                                pageRoles.Length == 1
                                && pageRoles[0] == resolvedPageRole)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.ImportPageRoleInfosAsync(
            appId: appId,
            pageRoleInfos: pageRoleInfos);

        // Then
        lookupProcessingServiceMock.Verify(
            expression: service =>
                service.ResolvePageRole(
                    appId: appId,
                    path: pageRoleInfo.Path,
                    roleName: pageRoleInfo.Role),
            times: Times.Exactly(callCount: 2));

        persistenceProcessingServiceMock.VerifyAll();
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

        lookupProcessingServiceMock.VerifyNoOtherCalls();
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

        lookupProcessingServiceMock
            .Setup(
                expression: service =>
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

        lookupProcessingServiceMock.VerifyAll();
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

        lookupProcessingServiceMock
            .Setup(
                expression: service =>
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

        lookupProcessingServiceMock.VerifyAll();
        persistenceProcessingServiceMock.VerifyNoOtherCalls();
    }
}