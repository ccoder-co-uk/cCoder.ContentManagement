// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace ContentManagement.IntegrationTests.Tests;

public sealed partial class PageEventTests
{
    [Fact]
    public async Task Post_GivenPageUpdateEvent_ShouldUpdatePageInfo()
    {
        int appId = await SeedAppAsync();

        try
        {
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId: appId);
            await SeedPageInfoAsync(page: page);

            await PostEventAsync(eventName: "page_update", data: CreatePageWithPageInfo(page: page, title: "Updated landing"));

            await WaitForAsync(
condition: () => HasPageInfo(pageId: page.Id, title: "Updated landing"),
because: "page_update should update the page info child row");

            HasPageInfo(pageId: page.Id, title: "Updated landing")
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageUpdateEvent_ShouldUpdateContent()
    {
        int appId = await SeedAppAsync();

        try
        {
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId: appId);
            await SeedContentAsync(page: page);

            await PostEventAsync(eventName: "page_update", data: CreatePageWithContent(page: page, html: "<p>Updated landing body</p>"));

            await WaitForAsync(
condition: () => HasContent(pageId: page.Id, html: "<p>Updated landing body</p>"),
because: "page_update should update the content child row");

            HasContent(pageId: page.Id, html: "<p>Updated landing body</p>")
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageUpdateEvent_ShouldUpdatePageRole()
    {
        int appId = await SeedAppAsync();

        try
        {
            Guid originalRoleId = await SeedRoleAsync(appId: appId);
            Guid updatedRoleId = await SeedRoleAsync(appId: appId);
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId: appId);
            await SeedPageRoleAsync(page: page, roleId: originalRoleId);

            await PostEventAsync(eventName: "page_update", data: CreatePageWithPageRole(page: page, roleId: updatedRoleId));

            await WaitForAsync(
condition: () => HasPageRole(pageId: page.Id, roleId: updatedRoleId) && !HasPageRole(pageId: page.Id, roleId: originalRoleId),
because: "page_update should update the page role child row");

            HasPageRole(pageId: page.Id, roleId: updatedRoleId)
                .Should()
                .BeTrue();

            HasPageRole(pageId: page.Id, roleId: originalRoleId)
                .Should()
                .BeFalse();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }
}