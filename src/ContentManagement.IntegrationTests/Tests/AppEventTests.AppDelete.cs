// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ContentManagement.IntegrationTests.Tests;

public sealed partial class AppEventTests
{
    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteAppCulture()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedAppCultureAsync(appId: appId);

            await PostEventAsync(eventName: "app_delete", data: new App { Id = appId });

            await WaitForAsync(
condition: () => HasNoAppCulture(appId: appId),
because: "app_delete should delete the app culture child row");

            using IServiceScope assertScope = Services.CreateScope();

            using CoreDataContext assertCore = assertScope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            assertCore.Set<App>()
                .IgnoreQueryFilters()
                .Any(predicate: app => app.Id == appId)
                .Should()
                .BeTrue();

            HasNoAppCulture(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteComponent()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedComponentAsync(appId: appId);

            await PostEventAsync(eventName: "app_delete", data: new App { Id = appId });

            await WaitForAsync(
condition: () => HasNoComponent(appId: appId),
because: "app_delete should delete the component child row");

            HasNoComponent(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteLayout()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedLayoutAsync(appId: appId);

            await PostEventAsync(eventName: "app_delete", data: new App { Id = appId });

            await WaitForAsync(
condition: () => HasNoLayout(appId: appId),
because: "app_delete should delete the layout child row");

            HasNoLayout(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeletePage()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedPageAsync(appId: appId);

            await PostEventAsync(eventName: "app_delete", data: new App { Id = appId });

            await WaitForAsync(
condition: () => HasNoPage(appId: appId),
because: "app_delete should delete the page child row");

            HasNoPage(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteResource()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedResourceAsync(appId: appId);

            await PostEventAsync(eventName: "app_delete", data: new App { Id = appId });

            await WaitForAsync(
condition: () => HasNoResource(appId: appId),
because: "app_delete should delete the resource child row");

            HasNoResource(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteScript()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedScriptAsync(appId: appId);

            await PostEventAsync(eventName: "app_delete", data: new App { Id = appId });

            await WaitForAsync(
condition: () => HasNoScript(appId: appId),
because: "app_delete should delete the script child row");

            HasNoScript(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteTemplate()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedTemplateAsync(appId: appId);

            await PostEventAsync(eventName: "app_delete", data: new App { Id = appId });

            await WaitForAsync(
condition: () => HasNoTemplate(appId: appId),
because: "app_delete should delete the template child row");

            HasNoTemplate(appId: appId)
                .Should()
                .BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }
}