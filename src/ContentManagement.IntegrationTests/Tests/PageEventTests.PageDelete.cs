using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;

namespace ContentManagement.IntegrationTests.Tests;

public sealed partial class PageEventTests
{
    [Fact]
    public async Task Post_GivenPageDeleteEvent_ShouldDeletePageInfo()
    {
        int appId = await SeedAppAsync();

        try
        {
            Page page = await SeedPageAsync(appId);
            await SeedPageInfoAsync(page);

            await PostEventAsync("page_delete", new Page { Id = page.Id });

            await WaitForAsync(
                () => HasNoPageInfo(page.Id),
                "page_delete should remove the page info child row");

            HasNoPageInfo(page.Id).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageDeleteEvent_ShouldDeleteContent()
    {
        int appId = await SeedAppAsync();

        try
        {
            Page page = await SeedPageAsync(appId);
            await SeedContentAsync(page);

            await PostEventAsync("page_delete", new Page { Id = page.Id });

            await WaitForAsync(
                () => HasNoContent(page.Id),
                "page_delete should remove the content child row");

            HasNoContent(page.Id).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageDeleteEvent_ShouldDeletePageRole()
    {
        int appId = await SeedAppAsync();

        try
        {
            Guid roleId = await SeedRoleAsync(appId);
            Page page = await SeedPageAsync(appId);
            await SeedPageRoleAsync(page, roleId);

            await PostEventAsync("page_delete", new Page { Id = page.Id });

            await WaitForAsync(
                () => HasNoPageRole(page.Id),
                "page_delete should remove the page role child row");

            HasNoPageRole(page.Id).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }
}
