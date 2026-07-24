// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;

namespace ContentManagement.IntegrationTests.Tests;

public sealed partial class PageEventTests
{
    [Fact]
    public async Task Post_GivenPageDeleteEvent_ShouldDeletePageInfo()
    {
        // Given
        int appId = await SeedAppAsync();

        try
        {
            Page page = await SeedPageAsync(appId: appId);
            await SeedPageInfoAsync(page: page);

            // When
            await PostEventAsync(eventName: "page_delete", data: new Page { Id = page.Id });

            // Then
            await WaitForAsync(
condition: () => HasNoPageInfo(pageId: page.Id),
because: "page_delete should remove the page info child row");

            HasNoPageInfo(pageId: page.Id)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageDeleteEvent_ShouldDeleteContent()
    {
        // Given
        int appId = await SeedAppAsync();

        try
        {
            Page page = await SeedPageAsync(appId: appId);
            await SeedContentAsync(page: page);

            // When
            await PostEventAsync(eventName: "page_delete", data: new Page { Id = page.Id });

            // Then
            await WaitForAsync(
condition: () => HasNoContent(pageId: page.Id),
because: "page_delete should remove the content child row");

            HasNoContent(pageId: page.Id)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageDeleteEvent_ShouldDeletePageRole()
    {
        // Given
        int appId = await SeedAppAsync();

        try
        {
            Guid roleId = await SeedRoleAsync(appId: appId);
            Page page = await SeedPageAsync(appId: appId);
            await SeedPageRoleAsync(page: page, roleId: roleId);

            // When
            await PostEventAsync(eventName: "page_delete", data: new Page { Id = page.Id });

            // Then
            await WaitForAsync(
condition: () => HasNoPageRole(pageId: page.Id),
because: "page_delete should remove the page role child row");

            HasNoPageRole(pageId: page.Id)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }
}