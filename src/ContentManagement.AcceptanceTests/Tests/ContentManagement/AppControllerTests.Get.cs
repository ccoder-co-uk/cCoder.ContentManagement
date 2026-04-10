using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_read");

        // When
        int actualCount = await GetAppCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);

        await Teardown(seededApp);
    }

    [Fact]
    public async Task Get_ReturnsSeededApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_read");
        App expectedApp = new() { Id = seededApp.AppId };

        // When
        App actualApp = await GetAppAsync(seededApp.AppId);

        // Then
        actualApp.Should().NotBeNull();
        actualApp!.Id.Should().Be(expectedApp.Id);

        await Teardown(seededApp);
    }

}






