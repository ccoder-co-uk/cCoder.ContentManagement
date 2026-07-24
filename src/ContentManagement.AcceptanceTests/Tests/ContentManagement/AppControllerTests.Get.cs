// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        SeededApp seededApp = await SeedDatabase(privileges: "app_read");

        // When
        int actualCount = await GetAppCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Get_ReturnsSeededApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_read");
        App expectedApp = new() { Id = seededApp.AppId };

        // When
        App actualApp = await GetAppAsync(id: seededApp.AppId);

        // Then

        actualApp.Should()
            .NotBeNull();

        actualApp!.Id.Should()
            .Be(expected: expectedApp.Id);

        await Teardown(seededApp: seededApp);
    }

}