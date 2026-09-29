// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseScriptDeleteEventAsync()
    {
        // Given
        Script entity = CreateRandomScript();

        scriptEventServiceMock
            .Setup(expression: x => x.RaiseScriptDeleteEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseScriptDeleteEventAsync(
            script: entity,
            userId: CurrentUserId);

        // Then
        scriptEventServiceMock.Verify(expression: x => x.RaiseScriptDeleteEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        scriptEventServiceMock.VerifyNoOtherCalls();
    }

}