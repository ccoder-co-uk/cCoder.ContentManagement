// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePageRoleAddEventAsync()
    {
        // Given
        PageRole entity = CreateRandomPageRole();

        pageRoleEventServiceMock
            .Setup(expression: x => x.RaisePageRoleAddEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePageRoleAddEventAsync(
            pageRole: entity,
            userId: CurrentUserId);

        // Then
        pageRoleEventServiceMock.Verify(expression: x => x.RaisePageRoleAddEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        pageRoleEventServiceMock.VerifyNoOtherCalls();
    }

}