using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageInfoControllerTests
{
    [Fact]
    public async Task Get_ReturnsListOfPageInfoRecords()
    {
        // Given

        // When
        IReadOnlyList<PageInfo> actualPageInfos = await GetPageInfosAsync(1);

        // Then
        actualPageInfos.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetPageInfoCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsPageInfoById()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase(includePageInfo: true);
        PageInfo actualPageInfo;

        // When
        actualPageInfo = await GetPageInfoAsync(seededContext.PageInfoId);

        // Then
        actualPageInfo.Should().NotBeNull();
        actualPageInfo!.Id.Should().Be(seededContext.PageInfoId);

        await DeletePageInfoAsync(seededContext.PageInfoId);
        await Teardown(seededContext);
    }
}






