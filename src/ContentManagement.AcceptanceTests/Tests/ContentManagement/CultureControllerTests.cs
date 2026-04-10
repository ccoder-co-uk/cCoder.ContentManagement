using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class CultureControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/Culture";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };
    private sealed record ODataEnvelope<T>(List<T> Value);

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededCultureContext(string CultureId);

    private async Task<SeededCultureContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();
        string cultureId = Unique("culture");

        await core.AddCultureAsync(new Culture
        {
            Id = cultureId,
            Name = Unique("Culture"),
        });

        return new SeededCultureContext(cultureId);
    }

    private async Task Teardown(params string[] cultureIds)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Culture[] cultures = core
            .Set<Culture>().IgnoreQueryFilters()
            .Where(culture => cultureIds.Contains(culture.Id))
            .ToArray();

        if (cultures.Length > 0)
            await core.DeleteAllAsync(cultures);
    }

    private async Task<Culture> CreateCultureAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<Culture>(content, JsonOptions)!;
    }

    private async Task<int> UpdateCultureAsync(string id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchCultureAsync(string id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}('{Uri.EscapeDataString(id)}')")
        {
            Content = JsonContent.Create(payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteCultureAsync(string id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<Culture> GetCultureAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<Culture>(content, JsonOptions);
    }

    private async Task<int> GetCultureCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<Culture>> GetCulturesAsync(int top = 1)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<Culture>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetCultureStatusCodeAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')");
        return (int)response.StatusCode;
    }
}







