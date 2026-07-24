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
    public async Task Post_GivenAppUpdateEvent_ShouldUpdateAppCulture()
    {
        // Given
        int appId = await SeedAppAsync();

        try
        {
            await SeedAppAdministratorAsync(appId: appId);
            await SeedAppCultureAsync(appId: appId);
            await SeedCultureAsync(cultureId: "fr-FR", name: "French");

            // When
            await PostEventAsync(eventName: "app_update", data: CreateAppWithAppCulture(appId: appId, cultureId: "fr-FR"));

            // Then
            await WaitForAsync(
condition: () =>
                {
                    using IServiceScope scope = Services.CreateScope();

                    using CoreDataContext core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                        .CreateCoreContext();

                    return core.Set<AppCulture>()
                        .IgnoreQueryFilters()
                        .Any(predicate: culture => culture.AppId == appId && culture.CultureId == "fr-FR")
                        && !core.Set<AppCulture>()
                        .IgnoreQueryFilters()
                        .Any(predicate: culture => culture.AppId == appId && culture.CultureId == "en-GB");
                },
because: "app_update should reconcile ContentManagement cultures");

            using IServiceScope assertScope = Services.CreateScope();

            using CoreDataContext assertCore = assertScope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            assertCore.Set<AppCulture>()
                .IgnoreQueryFilters()
                .Any(predicate: culture => culture.AppId == appId && culture.CultureId == "fr-FR")
                .Should()
                .BeTrue();

            assertCore.Set<AppCulture>()
                .IgnoreQueryFilters()
                .Any(predicate: culture => culture.AppId == appId && culture.CultureId == "en-GB")
                .Should()
                .BeFalse();
        }
        finally
        {
            await TeardownAppAsync(appId: appId);
        }
    }
}