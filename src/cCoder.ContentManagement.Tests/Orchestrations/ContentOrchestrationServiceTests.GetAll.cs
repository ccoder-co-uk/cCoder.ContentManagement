// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Content> entities = new[] { CreateRandomContent() }.AsQueryable();

        contentProcessingServiceMock.Setup(expression: x => x.GetAllContent(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllContent(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        contentProcessingServiceMock.Verify(expression: x => x.GetAllContent(ignoreFilters: true), times: Times.Once);
        contentProcessingServiceMock.VerifyNoOtherCalls();
        contentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}