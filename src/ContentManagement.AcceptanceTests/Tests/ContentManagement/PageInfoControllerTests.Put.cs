using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageInfoControllerTests
{
    [Fact]
    public async Task Put_UpdatesPageInfo()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase(includePageInfo: true);
        string updatedTitle = Unique("UpdatedTitle");
        PageInfo actualPageInfo;

        // When
        await UpdatePageInfoAsync(seededContext.PageInfoId, new
        {
            id = seededContext.PageInfoId,
            pageId = seededContext.PageId,
            cultureId = string.Empty,
            title = updatedTitle,
            description = "Updated page info",
            keywords = "updated",
        });

        actualPageInfo = await GetPageInfoAsync(seededContext.PageInfoId);

        // Then
        actualPageInfo.Should().NotBeNull();
        actualPageInfo!.Title.Should().Be(updatedTitle);

        await DeletePageInfoAsync(seededContext.PageInfoId);
        await Teardown(seededContext);
    }
}






