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
            await SeedAppAdministratorAsync(appId);
            await SeedAppCultureAsync(appId);

            await PostEventAsync("app_delete", new App { Id = appId });

            await WaitForAsync(
                () => HasNoAppCulture(appId),
                "app_delete should delete the app culture child row");

            using IServiceScope assertScope = Services.CreateScope();
            using CoreDataContext assertCore = assertScope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            assertCore.Set<App>().IgnoreQueryFilters()
                .Any(app => app.Id == appId)
                .Should().BeTrue();
            HasNoAppCulture(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteComponent()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedComponentAsync(appId);

            await PostEventAsync("app_delete", new App { Id = appId });

            await WaitForAsync(
                () => HasNoComponent(appId),
                "app_delete should delete the component child row");

            HasNoComponent(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteLayout()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedLayoutAsync(appId);

            await PostEventAsync("app_delete", new App { Id = appId });

            await WaitForAsync(
                () => HasNoLayout(appId),
                "app_delete should delete the layout child row");

            HasNoLayout(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeletePage()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedPageAsync(appId);

            await PostEventAsync("app_delete", new App { Id = appId });

            await WaitForAsync(
                () => HasNoPage(appId),
                "app_delete should delete the page child row");

            HasNoPage(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteResource()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedResourceAsync(appId);

            await PostEventAsync("app_delete", new App { Id = appId });

            await WaitForAsync(
                () => HasNoResource(appId),
                "app_delete should delete the resource child row");

            HasNoResource(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteScript()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedScriptAsync(appId);

            await PostEventAsync("app_delete", new App { Id = appId });

            await WaitForAsync(
                () => HasNoScript(appId),
                "app_delete should delete the script child row");

            HasNoScript(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }

    [Fact]
    public async Task Post_GivenAppDeleteEvent_ShouldDeleteTemplate()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedTemplateAsync(appId);

            await PostEventAsync("app_delete", new App { Id = appId });

            await WaitForAsync(
                () => HasNoTemplate(appId),
                "app_delete should delete the template child row");

            HasNoTemplate(appId).Should().BeTrue();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }
}
