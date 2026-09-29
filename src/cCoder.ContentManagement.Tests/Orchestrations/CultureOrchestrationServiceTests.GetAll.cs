// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Culture> entities = new[] { CreateRandomCulture() }.AsQueryable();

        cultureProcessingServiceMock.Setup(expression: x => x.GetAllCulture(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllCulture(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        cultureProcessingServiceMock.Verify(expression: x => x.GetAllCulture(ignoreFilters: true), times: Times.Once);
        cultureProcessingServiceMock.VerifyNoOtherCalls();
        cultureEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}