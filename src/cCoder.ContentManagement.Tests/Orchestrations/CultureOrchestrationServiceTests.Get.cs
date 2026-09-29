// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        string id = "en-GB";
        Culture entity = CreateRandomCulture();

        cultureProcessingServiceMock.Setup(expression: x => x.GetCulture(cultureId: id))
            .Returns(value: entity);

        // When
        Culture result = orchestrationService.GetCulture(cultureId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity);

        cultureProcessingServiceMock.Verify(expression: x => x.GetCulture(cultureId: id), times: Times.Once);
        cultureProcessingServiceMock.VerifyNoOtherCalls();
        cultureEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}