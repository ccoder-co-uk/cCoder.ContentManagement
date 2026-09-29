// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureServiceMock.Setup(expression: x => x.UpdateCultureAsync(updatedCulture: entity))
            .ReturnsAsync(value: entity);

        // When
        Culture result = await cultureProcessingService.UpdateCultureAsync(updatedCulture: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        cultureServiceMock.Verify(expression: x => x.UpdateCultureAsync(updatedCulture: entity), times: Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}