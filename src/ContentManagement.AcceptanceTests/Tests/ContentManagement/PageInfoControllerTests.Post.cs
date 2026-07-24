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
    public async Task Post_CreatesPageInfo()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase();
        string title = Unique(prefix: "Title");
        PageInfo expectedPageInfo;
        PageInfo actualPageInfo;

        // When

        expectedPageInfo = await CreatePageInfoAsync(payload: new
        {
            pageId = seededContext.PageId,
            cultureId = string.Empty,
            title,
            description = "Acceptance page info",
            keywords = "acceptance",
        });

        actualPageInfo = await GetPageInfoAsync(id: expectedPageInfo.Id);

        // Then

        actualPageInfo.Should()
            .NotBeNull();

        actualPageInfo!.Title.Should()
            .Be(expected: title);

        await DeletePageInfoAsync(id: expectedPageInfo.Id);
        await Teardown(seededContext: seededContext);
    }
}