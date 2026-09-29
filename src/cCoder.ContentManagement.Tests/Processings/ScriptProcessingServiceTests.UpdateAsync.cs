// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Script entity = CreateRandomScript();

        scriptServiceMock.Setup(expression: x => x.UpdateScriptAsync(updatedScript: entity))
            .ReturnsAsync(value: entity);

        // When
        Script result = await scriptProcessingService.UpdateScriptAsync(updatedScript: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        scriptServiceMock.Verify(expression: x => x.UpdateScriptAsync(updatedScript: entity), times: Times.Once);
        scriptServiceMock.VerifyNoOtherCalls();
    }

}