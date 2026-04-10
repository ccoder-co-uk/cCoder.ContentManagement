using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageInfoControllerTests
{
    [Fact]
    public async Task Patch_UpdatesPageInfo()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase(includePageInfo: true);
        string updatedTitle = Unique("PatchedTitle");
        PageInfo actualPageInfo;

        // When
        await PatchPageInfoAsync(seededContext.PageInfoId, new
        {
            title = updatedTitle,
            keywords = "patched",
        });

        actualPageInfo = await GetPageInfoAsync(seededContext.PageInfoId);

        // Then
        actualPageInfo.Should().NotBeNull();
        actualPageInfo!.Title.Should().Be(updatedTitle);
        actualPageInfo.Keywords.Should().Be("patched");

        await DeletePageInfoAsync(seededContext.PageInfoId);
        await Teardown(seededContext);
    }
}






