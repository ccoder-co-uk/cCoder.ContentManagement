// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetChildren()
    {
        // Given
        Page[] expected = [CreateRandomPage()];

        pageProcessingServiceMock.Setup(expression: x => x.GetChildrenPage(pageId: 1))
            .Returns(value: expected);

        // When
        var result = orchestrationService.GetChildrenPage(pageId: 1)
            .ToArray();

        // Then
        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: expected.Select(selector: item => item.Id));

        pageProcessingServiceMock.Verify(expression: x => x.GetChildrenPage(pageId: 1), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}