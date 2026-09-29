// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CultureProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Culture entity = CreateRandomCulture();
        var id = entity.Id;

        cultureServiceMock.Setup(expression: x => x.GetCulture(cultureId: id))
            .Returns(value: entity);

        // When
        Culture result = cultureProcessingService.GetCulture(cultureId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        cultureServiceMock.Verify(expression: x => x.GetCulture(cultureId: id), times: Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}