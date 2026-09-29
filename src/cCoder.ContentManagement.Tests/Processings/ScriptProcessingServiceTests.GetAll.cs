// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Script> entities = new[] { CreateRandomScript() }.AsQueryable();

        scriptServiceMock.Setup(expression: x => x.GetAllScript())
            .Returns(value: entities);

        // When
        IQueryable<Script> result = scriptProcessingService.GetAllScript();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        scriptServiceMock.Verify(expression: x => x.GetAllScript(), times: Times.Once);
        scriptServiceMock.VerifyNoOtherCalls();
    }

}