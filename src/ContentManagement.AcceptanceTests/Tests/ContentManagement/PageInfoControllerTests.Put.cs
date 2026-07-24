// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        string updatedTitle = Unique(prefix: "UpdatedTitle");
        PageInfo actualPageInfo;

        // When

        await UpdatePageInfoAsync(id: seededContext.PageInfoId, payload: new
        {
            id = seededContext.PageInfoId,
            pageId = seededContext.PageId,
            cultureId = string.Empty,
            title = updatedTitle,
            description = "Updated page info",
            keywords = "updated",
        });

        actualPageInfo = await GetPageInfoAsync(id: seededContext.PageInfoId);

        // Then

        actualPageInfo.Should()
            .NotBeNull();

        actualPageInfo!.Title.Should()
            .Be(expected: updatedTitle);

        await DeletePageInfoAsync(id: seededContext.PageInfoId);
        await Teardown(seededContext: seededContext);
    }
}