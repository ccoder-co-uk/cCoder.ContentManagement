// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace ContentManagement.IntegrationTests.Tests;

public sealed partial class PageEventTests
{
    [Fact]
    public async Task Post_GivenPageAddEvent_ShouldCreatePageInfo()
    {
        int appId = await SeedAppAsync();

        try
        {
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId: appId);

            await PostEventAsync(eventName: "page_add", data: CreatePageWithPageInfo(page: page, title: "Landing"));

            await WaitForAsync(
condition: () => HasPageInfo(pageId: page.Id, title: "Landing"),
because: "page_add should create the page info child row");

            HasPageInfo(pageId: page.Id, title: "Landing")
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageAddEvent_ShouldCreateContent()
    {
        int appId = await SeedAppAsync();

        try
        {
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId: appId);

            await PostEventAsync(eventName: "page_add", data: CreatePageWithContent(page: page, html: "<p>Landing body</p>"));

            await WaitForAsync(
condition: () => HasContent(pageId: page.Id, html: "<p>Landing body</p>"),
because: "page_add should create the content child row");

            HasContent(pageId: page.Id, html: "<p>Landing body</p>")
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageAddEvent_ShouldCreatePageRole()
    {
        int appId = await SeedAppAsync();

        try
        {
            Guid roleId = await SeedRoleAsync(appId: appId);
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId: appId);

            await PostEventAsync(eventName: "page_add", data: CreatePageWithPageRole(page: page, roleId: roleId));

            await WaitForAsync(
condition: () => HasPageRole(pageId: page.Id, roleId: roleId),
because: "page_add should create the page role child row");

            HasPageRole(pageId: page.Id, roleId: roleId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }
}