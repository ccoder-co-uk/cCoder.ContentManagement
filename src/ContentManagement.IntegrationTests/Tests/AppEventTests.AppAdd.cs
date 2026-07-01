using FluentAssertions;
using Xunit;

namespace ContentManagement.IntegrationTests.Tests;

public sealed partial class AppEventTests
{
    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateAppCulture()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedCultureAsync("en-GB", "English (UK)");

            await PostEventAsync("app_add", CreateAppWithAppCulture(appId));

            await WaitForAsync(
                () => HasAppCulture(appId),
                "app_add should create the app culture child row");

            HasAppCulture(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateComponent()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedCultureAsync("en-GB", "English (UK)");

            await PostEventAsync("app_add", CreateAppWithComponent(appId));

            await WaitForAsync(
                () => HasComponent(appId),
                "app_add should create the component child row");

            HasComponent(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateLayout()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedCultureAsync("en-GB", "English (UK)");

            await PostEventAsync("app_add", CreateAppWithLayout(appId));

            await WaitForAsync(
                () => HasLayout(appId),
                "app_add should create the layout child row");

            HasLayout(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreatePage()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedCultureAsync("en-GB", "English (UK)");

            await PostEventAsync("app_add", CreateAppWithPage(appId));

            await WaitForAsync(
                () => HasPage(appId),
                "app_add should create the page child row");

            HasPage(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateResource()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedCultureAsync("en-GB", "English (UK)");

            await PostEventAsync("app_add", CreateAppWithResource(appId));

            await WaitForAsync(
                () => HasResource(appId),
                "app_add should create the resource child row");

            HasResource(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateScript()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedCultureAsync("en-GB", "English (UK)");

            await PostEventAsync("app_add", CreateAppWithScript(appId));

            await WaitForAsync(
                () => HasScript(appId),
                "app_add should create the script child row");

            HasScript(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateTemplate()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedCultureAsync("en-GB", "English (UK)");

            await PostEventAsync("app_add", CreateAppWithTemplate(appId));

            await WaitForAsync(
                () => HasTemplate(appId),
                "app_add should create the template child row");

            HasTemplate(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }
}
