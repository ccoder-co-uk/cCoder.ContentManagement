// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    private string BaseUrl { get; } = "/Api/ContentManagement/Culture";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };
    private sealed record ODataEnvelope<T>(List<T> Value);

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededCultureContext(string CultureId);

    private async Task<SeededCultureContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        string cultureId = Unique(prefix: "culture");

        await core.AddCultureAsync(culture: new Culture
        {
            Id = cultureId,
            Name = Unique(prefix: "Culture"),
        });

        return new SeededCultureContext(CultureId: cultureId);
    }

    private async Task Teardown(params string[] cultureIds)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Culture[] cultures = core
            .Set<Culture>()
            .IgnoreQueryFilters()
            .Where(predicate: culture => cultureIds.Contains(value: culture.Id))
            .ToArray();

        if (cultures.Length > 0)
        {
            await core.DeleteAllAsync(cultures: cultures);
        }
    }

    private async Task<Culture> CreateCultureAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<Culture>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateCultureAsync(string id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchCultureAsync(string id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteCultureAsync(string id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<Culture> GetCultureAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        if (content.Contains(value: "\"value\":[]", comparisonType: StringComparison.Ordinal))
        {
            return null;
        }

        return JsonSerializer.Deserialize<Culture>(json: content, options: JsonOptions);
    }

    private async Task<int> GetCultureCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<Culture>> GetCulturesAsync(int top = 1)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<Culture>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> GetCultureStatusCodeAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')");
        return (int)response.StatusCode;
    }
}