// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ScriptServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Script[] expectedItems = [CreateRandomScript()];

        IQueryable<CmsDataModels.Script> scripts = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        scriptBrokerMock.Setup(expression: x => x.GetAllScripts())
            .Returns(value: scripts);

        // When
        IQueryable<Script> result = scriptService.GetAllScripts();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        scriptBrokerMock.Verify(expression: x => x.GetAllScripts(), times: Times.Once);
        scriptBrokerMock.VerifyNoOtherCalls();
}

}