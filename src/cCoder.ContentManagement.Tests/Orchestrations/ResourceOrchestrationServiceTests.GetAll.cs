// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ResourceOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Resource> entities = new[] { CreateRandomResource() }.AsQueryable();

        resourceProcessingServiceMock.Setup(expression: x => x.GetAllResource(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllResource(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        resourceProcessingServiceMock.Verify(expression: x => x.GetAllResource(ignoreFilters: true), times: Times.Once);
        resourceProcessingServiceMock.VerifyNoOtherCalls();
        resourceEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}