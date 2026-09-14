// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRoleOrchestrationServiceTests
{
    [Fact]
    public async Task AddPageRole_WhenAuthorizationFails_ShouldNotRaiseEventOrPersistAsync()
    {
        // Given
        PageRole pageRole = CreateRandomPageRole();

        SetupAuthorization(
            pageRole: pageRole,
            privilege: "pagerole_create",
            allowed: false,
            requireRole: true);

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddPageRoleAsync(
                newPageRole: pageRole);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        pageRoleEventProcessingServiceMock.Verify(
            expression: service =>
                service.RaisePageRoleAddEventAsync(
                    entity: pageRole,
                    userId: It.IsAny<string>()),
            times: Times.Never);

        pageRoleProcessingServiceMock.Verify(
            expression: service =>
                service.AddPageRoleAsync(newPageRole: pageRole),
            times: Times.Never);
    }

    [Fact]
    public async Task DeletePageRole_WhenAuthorizationFails_ShouldNotRaiseEventOrPersistAsync()
    {
        // Given
        PageRole pageRole = CreateRandomPageRole();

        SetupAuthorization(
            pageRole: pageRole,
            privilege: "pagerole_delete",
            allowed: false);

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeletePageRoleAsync(
                deletedPageRole: pageRole);

        // Then
        await action.Should()
            .ThrowAsync<ContentManagementSecurityException>();

        pageRoleEventProcessingServiceMock.Verify(
            expression: service =>
                service.RaisePageRoleDeleteEventAsync(
                    entity: pageRole,
                    userId: It.IsAny<string>()),
            times: Times.Never);

        pageRoleProcessingServiceMock.Verify(
            expression: service =>
                service.DeletePageRoleAsync(deletedPageRole: pageRole),
            times: Times.Never);
    }
}
