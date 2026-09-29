// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        PageRole entity = CreateRandomPageRole();

        SetupAuthorization(
            pageRole: entity,
            privilege: "pagerole_create",
            allowed: true,
            requireRole: true);

        pageRoleProcessingServiceMock.Setup(expression: x => x.AddPageRoleAsync(newPageRole: entity))
            .ReturnsAsync(value: entity);

        pageRoleEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageRoleAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageRole result = await orchestrationService.AddPageRoleAsync(newPageRole: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageRoleProcessingServiceMock.Verify(expression: x => x.AddPageRoleAsync(newPageRole: entity), times: Times.Once);
        pageRoleEventProcessingServiceMock.Verify(expression: x => x.RaisePageRoleAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

}