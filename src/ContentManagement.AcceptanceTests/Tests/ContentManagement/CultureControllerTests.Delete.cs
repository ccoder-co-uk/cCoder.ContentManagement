// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CultureControllerTests
{
    [Fact]
    public async Task Delete_RemovesCulture()
    {
        // Given
        SeededCultureContext seededContext = await SeedDatabase();

        // When
        int actualStatusCode = await DeleteCultureAsync(id: seededContext.CultureId);
        int actualReadStatusCode = await GetCultureStatusCodeAsync(id: seededContext.CultureId);

        // Then

        actualStatusCode.Should()
            .Be(expected: 204);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(cultureIds: seededContext.CultureId);
    }
}