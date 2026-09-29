// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        App entity = CreateRandomApp();

        appProcessingServiceMock.Setup(expression: x => x.GetApp(appId: id))
            .Returns(value: entity);

        // When
        App result = orchestrationService.GetApp(appId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity, config: options => options.Excluding(expression: app => app.Config));

        appProcessingServiceMock.Verify(expression: x => x.GetApp(appId: id), times: Times.Once);
        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}