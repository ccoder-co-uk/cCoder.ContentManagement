// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Component> entities = new[] { CreateRandomComponent() }.AsQueryable();

        componentProcessingServiceMock.Setup(expression: x => x.GetAllComponents(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllComponents(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        componentProcessingServiceMock.Verify(expression: x => x.GetAllComponents(ignoreFilters: true), times: Times.Once);
        componentProcessingServiceMock.VerifyNoOtherCalls();
        componentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}