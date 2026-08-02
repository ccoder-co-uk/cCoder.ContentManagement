// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Delete_RemovesApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_delete");

        // When
        int actualStatusCode = await DeleteAppAsync(host: seededApp.Domain, id: seededApp.AppId);
        int actualReadStatusCode = await GetAppStatusCodeAsync(host: seededApp.Domain, id: seededApp.AppId);

        // Then

        actualStatusCode.Should()
            .Be(expected: 202);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(seededApp: seededApp);
    }
}