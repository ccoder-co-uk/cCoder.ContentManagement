// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class ContentControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/ContentManagement/Content";

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    [Fact]
    public async Task Get_WithPageExpansion_ReturnsPage()
    {
        // Given
        App app;
        Page page;
        Content contentEntity;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            app = await core.AddAppAsync(app: new App
            {
                Name = Unique(prefix: "ContentExpansionApp"),
                Domain = $"{Unique(prefix: "content-expansion")}.local",
                DefaultTheme = "Default",
                DefaultCultureId = string.Empty,
                TenantId = Unique(prefix: "tenant"),
                ConfigJson = "{}",
            });

            page = await core.AddPageAsync(page: new Page
            {
                AppId = app.Id,
                Name = Unique(prefix: "ContentExpansionPage"),
                Path = Unique(prefix: "content-expansion-page"),
                Layout = string.Empty,
                ShowOnMenus = true,
                Order = 1,
            });

            contentEntity = await core.AddContentAsync(content: new Content
            {
                PageId = page.Id,
                CultureId = string.Empty,
                Name = "body",
                Html = "<p>Content expansion</p>",
            });
        }

        // When
        using HttpResponseMessage response = await Client.GetAsync(
requestUri: $"{BaseUrl}({contentEntity.Id})?$expand=Page");

        string responseContent = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: responseContent);

        JsonObject actualContent = JsonNode.Parse(json: responseContent)!.AsObject();

        actualContent["Page"]!.AsObject()["Id"]!.GetValue<int>()
            .Should()
            .Be(expected: page.Id);

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            core.Contents.Remove(entity: contentEntity);
            core.Pages.Remove(entity: page);
            core.Apps.Remove(entity: app);
            _ = await core.SaveChangesAsync();
        }
    }
}