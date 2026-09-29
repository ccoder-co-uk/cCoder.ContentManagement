// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        PageRole pageRole = CreateRandomPageRole();

        SetupAuthorization(
            pageRole: pageRole,
            privilege: "pagerole_delete",
            allowed: true);

        pageRoleProcessingServiceMock.Setup(expression: x => x.DeletePageRoleAsync(deletedPageRole: pageRole))
            .Returns(value: ValueTask.CompletedTask);

        pageRoleEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageRoleDeleteEventAsync(entity: pageRole, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeletePageRoleAsync(deletedPageRole: pageRole);

        // Then
        pageRoleProcessingServiceMock.Verify(expression: x => x.DeletePageRoleAsync(deletedPageRole: pageRole), times: Times.Once);
        pageRoleEventProcessingServiceMock.Verify(expression: x => x.RaisePageRoleDeleteEventAsync(entity: pageRole, userId: CurrentUserId), times: Times.Once);
    }

}