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
    public async Task Patch_UpdatesPageInfo()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase(includePageInfo: true);
        string updatedTitle = Unique(prefix: "PatchedTitle");
        PageInfo actualPageInfo;

        // When

        await PatchPageInfoAsync(id: seededContext.PageInfoId, payload: new
        {
            title = updatedTitle,
            keywords = "patched",
        });

        actualPageInfo = await GetPageInfoAsync(id: seededContext.PageInfoId);

        // Then

        actualPageInfo.Should()
            .NotBeNull();

        actualPageInfo!.Title.Should()
            .Be(expected: updatedTitle);

        actualPageInfo.Keywords.Should()
            .Be(expected: "patched");

        await DeletePageInfoAsync(id: seededContext.PageInfoId);
        await Teardown(seededContext: seededContext);
    }
}