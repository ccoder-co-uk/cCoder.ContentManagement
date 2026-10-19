// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_read");

        // When
        int actualCount = await GetAppCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Get_ReturnsSeededApp()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: "app_read");
        App expectedApp = new() { Id = seededApp.AppId };

        // When
        App actualApp = await GetAppAsync(id: seededApp.AppId);

        // Then

        actualApp.Should()
            .NotBeNull();

        actualApp!.Id.Should()
            .Be(expected: expectedApp.Id);

        await Teardown(seededApp: seededApp);
    }

    [Fact]
    public async Task Get_WithLayoutsExpansion_ReturnsLayouts()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(
            privileges: ["app_read", "app_admin", "layout_read"]);

        Layout layout;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            layout = await core.AddLayoutAsync(layout: new Layout
            {
                AppId = seededApp.AppId,
                Name = Unique(prefix: "ExpansionLayout"),
                Description = "Expansion layout",
                HeaderHtml = "<title>Expansion</title>",
                Html = "<main>Expansion</main>",
                Script = string.Empty,
            });
        }

        // When
        using var response = await Client.GetAsync(
requestUri: $"{BaseUrl}({seededApp.AppId})?$expand=Layouts");

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        JsonObject actualApp = JsonNode.Parse(json: content)!.AsObject();

        actualApp["Layouts"]!.AsArray()
            .Should()
            .ContainSingle();

        actualApp["Layouts"]!.AsArray()[0]!["Id"]!.GetValue<int>()
            .Should()
            .Be(expected: layout.Id);

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            await core.DeleteAsync(layout: layout);
        }

        await Teardown(seededApp: seededApp);
    }

}