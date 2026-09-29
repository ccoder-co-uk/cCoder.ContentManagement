// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Template entity = CreateRandomTemplate();

        templateProcessingServiceMock.Setup(expression: x => x.GetTemplate(templateId: id))
            .Returns(value: entity);

        // When
        Template result = orchestrationService.GetTemplate(templateId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity);

        templateProcessingServiceMock.Verify(expression: x => x.GetTemplate(templateId: id), times: Times.Once);
        templateProcessingServiceMock.VerifyNoOtherCalls();
        templateEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}