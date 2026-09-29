// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Culture culture = CreateRandomCulture(id: "en-GB");

        cultureBrokerMock.Setup(expression: x => x.GetAllCultures())
            .Returns(value: new[] { culture }.AsQueryable());

        cultureBrokerMock.Setup(expression: x => x.DeleteCultureAsync(
                deletedCulture: It.Is<Culture>(match: deletedCulture =>
                    deletedCulture.Id == culture.Id &&
                    deletedCulture.Name == culture.Name)))
            .ReturnsAsync(value: 1);

        // When
        await cultureService.DeleteAsync(cultureId: "en-GB");

        // Then
        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(), times: Times.Once);

        cultureBrokerMock.Verify(
            expression: x => x.DeleteCultureAsync(
                deletedCulture: It.Is<Culture>(match: deletedCulture =>
                    deletedCulture.Id == culture.Id &&
                    deletedCulture.Name == culture.Name)),
            times: Times.Once);

        cultureBrokerMock.VerifyNoOtherCalls();
    }

}