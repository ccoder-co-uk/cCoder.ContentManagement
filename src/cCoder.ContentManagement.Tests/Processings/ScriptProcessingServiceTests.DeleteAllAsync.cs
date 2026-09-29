// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        Script entity = CreateRandomScript();
        var id = entity.Id;

        scriptServiceMock.Setup(expression: x => x.DeleteAsync(scriptId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await scriptProcessingService.DeleteAllScriptAsync(deletedScript: new[] { entity });

        // Then
        scriptServiceMock.Verify(expression: x => x.DeleteAsync(scriptId: id), times: Times.Once);
        scriptServiceMock.VerifyNoOtherCalls();
    }

}