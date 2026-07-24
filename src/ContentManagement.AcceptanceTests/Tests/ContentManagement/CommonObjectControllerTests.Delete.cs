// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Delete_RemovesCommonObject()
    {
        // Given
        SeededCommonObjectContext seededContext = await SeedDatabase();

        // When
        int actualStatusCode = await DeleteCommonObjectAsync(id: seededContext.Id);
        int actualReadStatusCode = await GetCommonObjectStatusCodeAsync(id: seededContext.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(ids: seededContext.Id);
    }
}