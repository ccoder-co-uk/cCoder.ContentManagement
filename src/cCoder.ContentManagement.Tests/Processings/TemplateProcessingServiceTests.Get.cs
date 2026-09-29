// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Template entity = CreateRandomTemplate();
        var id = entity.Id;

        templateServiceMock.Setup(expression: x => x.GetTemplate(templateId: id))
            .Returns(value: entity);

        // When
        Template result = templateProcessingService.GetTemplate(templateId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        templateServiceMock.Verify(expression: x => x.GetTemplate(templateId: id), times: Times.Once);
        templateServiceMock.VerifyNoOtherCalls();
    }

}