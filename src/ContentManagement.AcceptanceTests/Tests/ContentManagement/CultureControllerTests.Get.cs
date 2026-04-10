using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CultureControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetCultureCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCultures()
    {
        // Given

        // When
        IReadOnlyList<cCoder.Data.Models.CMS.Culture> actualCultures =
            await GetCulturesAsync();

        // Then
        actualCultures.Should().NotBeNull();
    }
}




