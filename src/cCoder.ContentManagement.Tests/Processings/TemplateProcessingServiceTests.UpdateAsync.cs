// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();

        templateServiceMock.Setup(expression: x => x.UpdateTemplateAsync(updatedTemplate: entity))
            .ReturnsAsync(value: entity);

        // When
        Template result = await templateProcessingService.UpdateTemplateAsync(updatedTemplate: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        templateServiceMock.Verify(expression: x => x.UpdateTemplateAsync(updatedTemplate: entity), times: Times.Once);
        templateServiceMock.VerifyNoOtherCalls();
    }

}