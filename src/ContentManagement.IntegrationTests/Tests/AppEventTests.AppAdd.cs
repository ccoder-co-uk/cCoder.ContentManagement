// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            await SeedAppAdministratorAsync(appId: appId);
            await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");

            await PostEventAsync(eventName: "app_add", data: CreateAppWithAppCulture(appId: appId));

            await WaitForAsync(
condition: () => HasAppCulture(appId: appId),
because: "app_add should create the app culture child row");

            HasAppCulture(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateComponent()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");

            await PostEventAsync(eventName: "app_add", data: CreateAppWithComponent(appId: appId));

            await WaitForAsync(
condition: () => HasComponent(appId: appId),
because: "app_add should create the component child row");

            HasComponent(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateLayout()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");

            await PostEventAsync(eventName: "app_add", data: CreateAppWithLayout(appId: appId));

            await WaitForAsync(
condition: () => HasLayout(appId: appId),
because: "app_add should create the layout child row");

            HasLayout(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreatePage()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");

            await PostEventAsync(eventName: "app_add", data: CreateAppWithPage(appId: appId));

            await WaitForAsync(
condition: () => HasPage(appId: appId),
because: "app_add should create the page child row");

            HasPage(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateResource()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");

            await PostEventAsync(eventName: "app_add", data: CreateAppWithResource(appId: appId));

            await WaitForAsync(
condition: () => HasResource(appId: appId),
because: "app_add should create the resource child row");

            HasResource(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateScript()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");

            await PostEventAsync(eventName: "app_add", data: CreateAppWithScript(appId: appId));

            await WaitForAsync(
condition: () => HasScript(appId: appId),
because: "app_add should create the script child row");

            HasScript(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppAddEvent_ShouldCreateTemplate()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedCultureAsync(cultureId: "en-GB", name: "English (UK)");

            await PostEventAsync(eventName: "app_add", data: CreateAppWithTemplate(appId: appId));

            await WaitForAsync(
condition: () => HasTemplate(appId: appId),
because: "app_add should create the template child row");

            HasTemplate(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }
}