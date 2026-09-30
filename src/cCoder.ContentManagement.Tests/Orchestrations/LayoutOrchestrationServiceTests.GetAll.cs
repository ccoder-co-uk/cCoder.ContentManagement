// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Layout> entities = new[] { CreateRandomLayout() }.AsQueryable();

        layoutProcessingServiceMock.Setup(expression: x => x.GetAllLayouts(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllLayouts(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        layoutProcessingServiceMock.Verify(expression: x => x.GetAllLayouts(ignoreFilters: true), times: Times.Once);
        layoutProcessingServiceMock.VerifyNoOtherCalls();
        layoutEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}