using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Delete_RemovesApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase("app_delete");

        // When
        int actualStatusCode = await DeleteAppAsync(seededApp.Domain, seededApp.AppId);
        int actualReadStatusCode = await GetAppStatusCodeAsync(seededApp.Domain, seededApp.AppId);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededApp);
    }
}



