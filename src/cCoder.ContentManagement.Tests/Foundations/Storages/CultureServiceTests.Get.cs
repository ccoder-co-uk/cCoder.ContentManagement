// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Culture culture = CreateRandomCulture(id: "en-GB");

        cultureBrokerMock.Setup(expression: x => x.GetAllCultures())
            .Returns(value: new[] { culture }.AsQueryable());

        // When
        Culture result = cultureService.GetCulture(cultureId: "en-GB");

        // Then

        result.Should()
            .BeEquivalentTo(expectation: culture);

        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(), times: Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
    }

}