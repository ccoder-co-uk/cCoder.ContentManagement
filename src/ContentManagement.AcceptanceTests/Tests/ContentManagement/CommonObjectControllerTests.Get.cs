// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetCommonObjectCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCommonObjects()
    {
        // Given

        // When
        IReadOnlyList<CommonObject> actualCommonObjects = await GetCommonObjectsAsync();

        // Then

        actualCommonObjects.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsCommonObjectById()
    {
        // Given
        SeededCommonObjectContext seededContext = await SeedDatabase();
        CommonObject expectedCommonObject = new() { Id = seededContext.Id };

        // When
        CommonObject actualCommonObject = await GetCommonObjectAsync(id: seededContext.Id);

        // Then

        actualCommonObject.Should()
            .NotBeNull();

        actualCommonObject!.Id.Should()
            .Be(expected: expectedCommonObject.Id);

        await DeleteCommonObjectAsync(id: seededContext.Id);
        await Teardown(ids: seededContext.Id);
    }

    [Theory]
    [InlineData("Core/Resource")]
    [InlineData("Core/Component")]
    [InlineData("Core/Script")]
    public async Task Latest_ReturnsSeededCacheEntries(string type)
    {
        // Given

        // When
        IReadOnlyList<CommonObject> actualCommonObjects = await GetLatestCommonObjectsAsync(type: type);

        // Then

        actualCommonObjects.Count.Should()
            .BeGreaterThan(expected: 0);
    }
}