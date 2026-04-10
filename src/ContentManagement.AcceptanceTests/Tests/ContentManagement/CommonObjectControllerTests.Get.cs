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
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCommonObjects()
    {
        // Given

        // When
        IReadOnlyList<CommonObject> actualCommonObjects = await GetCommonObjectsAsync();

        // Then
        actualCommonObjects.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsCommonObjectById()
    {
        // Given
        SeededCommonObjectContext seededContext = await SeedDatabase();
        CommonObject expectedCommonObject = new() { Id = seededContext.Id };

        // When
        CommonObject actualCommonObject = await GetCommonObjectAsync(seededContext.Id);

        // Then
        actualCommonObject.Should().NotBeNull();
        actualCommonObject!.Id.Should().Be(expectedCommonObject.Id);

        await DeleteCommonObjectAsync(seededContext.Id);
        await Teardown(seededContext.Id);
    }

    [Theory]
    [InlineData("Core/Resource")]
    [InlineData("Core/Component")]
    [InlineData("Core/Script")]
    public async Task Latest_ReturnsSeededCacheEntries(string type)
    {
        // Given

        // When
        IReadOnlyList<CommonObject> actualCommonObjects = await GetLatestCommonObjectsAsync(type);

        // Then
        actualCommonObjects.Count.Should().BeGreaterThan(0);
    }
}






