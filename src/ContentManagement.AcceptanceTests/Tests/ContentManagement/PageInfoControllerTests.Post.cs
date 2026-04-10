using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageInfoControllerTests
{
    [Fact]
    public async Task Post_CreatesPageInfo()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase();
        string title = Unique("Title");
        PageInfo expectedPageInfo;
        PageInfo actualPageInfo;

        // When
        expectedPageInfo = await CreatePageInfoAsync(new
        {
            pageId = seededContext.PageId,
            cultureId = string.Empty,
            title,
            description = "Acceptance page info",
            keywords = "acceptance",
        });

        actualPageInfo = await GetPageInfoAsync(expectedPageInfo.Id);

        // Then
        actualPageInfo.Should().NotBeNull();
        actualPageInfo!.Title.Should().Be(title);

        await DeletePageInfoAsync(expectedPageInfo.Id);
        await Teardown(seededContext);
    }
}






