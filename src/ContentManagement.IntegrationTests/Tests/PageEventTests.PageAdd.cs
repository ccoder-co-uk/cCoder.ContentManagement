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
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId);

            await PostEventAsync("page_add", CreatePageWithPageInfo(page, "Landing"));

            await WaitForAsync(
                () => HasPageInfo(page.Id, "Landing"),
                "page_add should create the page info child row");

            HasPageInfo(page.Id, "Landing").Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageAddEvent_ShouldCreateContent()
    {
        int appId = await SeedAppAsync();

        try
        {
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId);

            await PostEventAsync("page_add", CreatePageWithContent(page, "<p>Landing body</p>"));

            await WaitForAsync(
                () => HasContent(page.Id, "<p>Landing body</p>"),
                "page_add should create the content child row");

            HasContent(page.Id, "<p>Landing body</p>").Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenPageAddEvent_ShouldCreatePageRole()
    {
        int appId = await SeedAppAsync();

        try
        {
            Guid roleId = await SeedRoleAsync(appId);
            cCoder.Data.Models.CMS.Page page = await SeedPageAsync(appId);

            await PostEventAsync("page_add", CreatePageWithPageRole(page, roleId));

            await WaitForAsync(
                () => HasPageRole(page.Id, roleId),
                "page_add should create the page role child row");

            HasPageRole(page.Id, roleId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }
}
