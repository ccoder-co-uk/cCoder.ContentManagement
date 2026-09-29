// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<PageInfo> entities = new[] { CreateRandomPageInfo() }.AsQueryable();

        pageInfoProcessingServiceMock.Setup(expression: x => x.GetAllPageInfo(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<PageInfo> result = orchestrationService.GetAllPageInfo(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        pageInfoProcessingServiceMock.Verify(expression: x => x.GetAllPageInfo(ignoreFilters: true), times: Times.Once);
        pageInfoProcessingServiceMock.VerifyNoOtherCalls();
        pageInfoEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}