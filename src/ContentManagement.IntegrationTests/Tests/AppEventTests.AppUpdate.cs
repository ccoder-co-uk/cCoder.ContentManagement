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
    public async Task Post_GivenAppUpdateEvent_ShouldUpdateAppCulture()
    {
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId);
            await SeedAppCultureAsync(appId);
            await SeedCultureAsync("fr-FR", "French");

            await PostEventAsync("app_update", CreateAppWithAppCulture(appId, "fr-FR"));

            await WaitForAsync(
                () =>
                {
                    using IServiceScope scope = Services.CreateScope();
                    using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                        .CreateCoreContext();

                    return core.Set<AppCulture>().IgnoreQueryFilters()
                        .Any(culture => culture.AppId == appId && culture.CultureId == "fr-FR")
                        && !core.Set<AppCulture>().IgnoreQueryFilters()
                            .Any(culture => culture.AppId == appId && culture.CultureId == "en-GB");
                },
                "app_update should reconcile ContentManagement cultures");

            using IServiceScope assertScope = Services.CreateScope();
            using CoreDataContext assertCore = assertScope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            assertCore.Set<AppCulture>().IgnoreQueryFilters()
                .Any(culture => culture.AppId == appId && culture.CultureId == "fr-FR")
                .Should().BeTrue();
            assertCore.Set<AppCulture>().IgnoreQueryFilters()
                .Any(culture => culture.AppId == appId && culture.CultureId == "en-GB")
                .Should().BeFalse();
        }
        finally
        {
            await TeardownAppAsync(appId);
        }
    }
}
