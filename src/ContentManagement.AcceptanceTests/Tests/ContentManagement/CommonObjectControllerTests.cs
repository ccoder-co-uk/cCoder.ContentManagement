using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class CommonObjectControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/CommonObject";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };
    private sealed record ODataEnvelope<T>(List<T> Value);

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededCommonObjectContext(
        int Id,
        string Name,
        string Key,
        string Type,
        string Culture
    );

    private async Task<SeededCommonObjectContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        string key = Unique("key");
        const string type = "Acceptance/Test";
        string name = Unique("CommonObject");
        DateTimeOffset now = DateTimeOffset.UtcNow;
        CommonObject commonObject = await core.AddCommonObjectAsync(new CommonObject
        {
            Name = name,
            Description = "Acceptance common object",
            LastUpdated = now,
            LastUpdatedBy = "Guest",
            CreatedOn = now,
            CreatedBy = "Guest",
            Version = 1,
            Key = key,
            Type = type,
            Json = "{\"enabled\":true}",
            Culture = string.Empty,
        });

        return new SeededCommonObjectContext(commonObject.Id, name, key, type, string.Empty);
    }

    private async Task Teardown(params int[] ids)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        CommonObject[] commonObjects = core
            .Set<CommonObject>().IgnoreQueryFilters()
            .Where(commonObject => ids.Contains(commonObject.Id))
            .ToArray();

        if (commonObjects.Length > 0)
            await core.DeleteAllAsync(commonObjects);
    }

    private async Task<CommonObject> CreateCommonObjectAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<CommonObject>(content, JsonOptions)!;
    }

    private async Task<int> UpdateCommonObjectAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchCommonObjectAsync(int id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteCommonObjectAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<CommonObject> GetCommonObjectAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<CommonObject>(content, JsonOptions);
    }

    private async Task<int> GetCommonObjectCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<CommonObject>> GetCommonObjectsAsync(int top = 1)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<CommonObject>>(content, JsonOptions)!.Value;
    }

    private async Task<IReadOnlyList<CommonObject>> GetLatestCommonObjectsAsync(string type)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/Latest()?type={Uri.EscapeDataString(type)}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        if (content.Contains("\"value\":", StringComparison.OrdinalIgnoreCase))
            return JsonSerializer.Deserialize<ODataEnvelope<CommonObject>>(content, JsonOptions)!.Value;

        return JsonSerializer.Deserialize<List<CommonObject>>(content, JsonOptions)!;
    }

    private async Task<string> ImportCommonObjectsAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync($"{BaseUrl}/Import", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return content;
    }

    private async Task<IReadOnlyList<CommonObject>> FilterCommonObjectsByKeyAsync(string key)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$filter=Key eq '{key}'");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<CommonObject>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetCommonObjectStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}







