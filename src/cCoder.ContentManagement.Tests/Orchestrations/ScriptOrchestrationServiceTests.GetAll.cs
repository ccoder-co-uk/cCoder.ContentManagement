// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ScriptOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Script> entities = new[] { CreateRandomScript() }.AsQueryable();

        scriptProcessingServiceMock.Setup(expression: x => x.GetAllScripts(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllScripts(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        scriptProcessingServiceMock.Verify(expression: x => x.GetAllScripts(ignoreFilters: true), times: Times.Once);
        scriptProcessingServiceMock.VerifyNoOtherCalls();
        scriptEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}