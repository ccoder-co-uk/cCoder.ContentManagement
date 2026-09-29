// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ScriptOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Script[] entities = [CreateRandomScript()];

        scriptProcessingServiceMock.Setup(expression: x => x.GetScript(scriptId: entities[0].Id))
            .Returns(value: entities[0]);

        scriptEventProcessingServiceMock.Setup(expression: x => x.RaiseScriptDeleteEventAsync(entity: entities[0], userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        scriptProcessingServiceMock.Setup(expression: x => x.DeleteAsync(scriptId: entities[0].Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllScriptAsync(deletedScript: entities);

        // Then
        scriptProcessingServiceMock.Verify(expression: x => x.GetScript(scriptId: entities[0].Id), times: Times.Once);
        scriptEventProcessingServiceMock.Verify(expression: x => x.RaiseScriptDeleteEventAsync(entity: entities[0], userId: CurrentUserId), times: Times.Once);
        scriptProcessingServiceMock.Verify(expression: x => x.DeleteAsync(scriptId: entities[0].Id), times: Times.Once);
    }

}