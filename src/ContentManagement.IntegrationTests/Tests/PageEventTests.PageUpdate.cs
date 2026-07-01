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
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId);
            await SeedPageInfoAsync(page);

            await PostEventAsync("page_update", CreatePageWithPageInfo(page, "Updated landing"));

            await WaitForAsync(
                () => HasPageInfo(page.Id, "Updated landing"),
                "page_update should update the page info child row");

            HasPageInfo(page.Id, "Updated landing").Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageUpdateEvent_ShouldUpdateContent()
    {
        int appId = await SeedAppAsync();

        try
        {
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId);
            await SeedContentAsync(page);

            await PostEventAsync("page_update", CreatePageWithContent(page, "<p>Updated landing body</p>"));

            await WaitForAsync(
                () => HasContent(page.Id, "<p>Updated landing body</p>"),
                "page_update should update the content child row");

            HasContent(page.Id, "<p>Updated landing body</p>").Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageUpdateEvent_ShouldUpdatePageRole()
    {
        int appId = await SeedAppAsync();

        try
        {
            Guid originalRoleId = await SeedRoleAsync(appId);
            Guid updatedRoleId = await SeedRoleAsync(appId);
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId);
            await SeedPageRoleAsync(page, originalRoleId);

            await PostEventAsync("page_update", CreatePageWithPageRole(page, updatedRoleId));

            await WaitForAsync(
                () => HasPageRole(page.Id, updatedRoleId) && !HasPageRole(page.Id, originalRoleId),
                "page_update should update the page role child row");

            HasPageRole(page.Id, updatedRoleId).Should().BeTrue();
            HasPageRole(page.Id, originalRoleId).Should().BeFalse();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }
}
