// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CultureControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetCultureCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCultures()
    {
        // Given

        // When
        IReadOnlyList<cCoder.Data.Models.CMS.Culture> actualCultures =
            await GetCulturesAsync();

        // Then

        actualCultures.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_WithAppsExpansion_ReturnsApps()
    {
        // Given
        SeededCultureContext seededContext = await SeedDatabase();
        App app;
        AppCulture appCulture;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            app = await core.AddAppAsync(app: new App
            {
                Name = Unique(prefix: "ExpansionApp"),
                Domain = $"{Unique(prefix: "expansion")}.local",
                DefaultTheme = "Default",
                DefaultCultureId = seededContext.CultureId,
                TenantId = Unique(prefix: "tenant"),
                ConfigJson = "{}",
            });

            appCulture = await core.AddAppCultureAsync(appCulture: new AppCulture
            {
                AppId = app.Id,
                CultureId = seededContext.CultureId,
            });
        }

        // When
        using var response = await Client.GetAsync(
requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: seededContext.CultureId)}')?$expand=Apps");

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        JsonObject actualCulture = JsonNode.Parse(json: content)!.AsObject();

        actualCulture["Apps"]!.AsArray()
            .Should()
            .ContainSingle();

        actualCulture["Apps"]!.AsArray()[0]!["AppId"]!.GetValue<int>()
            .Should()
            .Be(expected: app.Id);

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            core.AppCultures.Remove(entity: appCulture);
            core.Apps.Remove(entity: app);
            _ = await core.SaveChangesAsync();
        }

        await Teardown(cultureIds: seededContext.CultureId);
    }
}