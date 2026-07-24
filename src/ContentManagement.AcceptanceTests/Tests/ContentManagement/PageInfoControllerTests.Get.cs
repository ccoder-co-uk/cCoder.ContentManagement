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
    public async Task Get_ReturnsListOfPageInfoRecords()
    {
        // Given

        // When
        IReadOnlyList<PageInfo> actualPageInfos = await GetPageInfosAsync(top: 1);

        // Then

        actualPageInfos.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetPageInfoCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsPageInfoById()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase(includePageInfo: true);
        PageInfo actualPageInfo;

        // When
        actualPageInfo = await GetPageInfoAsync(id: seededContext.PageInfoId);

        // Then

        actualPageInfo.Should()
            .NotBeNull();

        actualPageInfo!.Id.Should()
            .Be(expected: seededContext.PageInfoId);

        await DeletePageInfoAsync(id: seededContext.PageInfoId);
        await Teardown(seededContext: seededContext);
    }
}