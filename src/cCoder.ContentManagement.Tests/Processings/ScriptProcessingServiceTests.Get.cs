// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Script entity = CreateRandomScript();
        var id = entity.Id;

        scriptServiceMock.Setup(expression: x => x.GetScript(scriptId: id))
            .Returns(value: entity);

        // When
        Script result = scriptProcessingService.GetScript(scriptId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        scriptServiceMock.Verify(expression: x => x.GetScript(scriptId: id), times: Times.Once);
        scriptServiceMock.VerifyNoOtherCalls();
    }

}