// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Script script = CreateRandomScript(id: 7);

        scriptBrokerMock.Setup(expression: x => x.GetAllScripts())
            .Returns(value: new[] { script }.AsQueryable());

        // When
        Script result = scriptService.GetScript(scriptId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: script);

        scriptBrokerMock.Verify(expression: x => x.GetAllScripts(), times: Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
}

}