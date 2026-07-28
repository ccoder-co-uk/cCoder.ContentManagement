// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    private string BaseUrl { get; } = "/Api/ContentManagement/CommonObject";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };
    private sealed record ODataEnvelope<T>(List<T> Value);

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

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

        string key = Unique(prefix: "key");
        const string type = "Acceptance/Test";
        string name = Unique(prefix: "CommonObject");
        DateTimeOffset now = DateTimeOffset.UtcNow;

        CommonObject commonObject = await core.AddCommonObjectAsync(commonObject: new CommonObject
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

        return new SeededCommonObjectContext(Id: commonObject.Id, Name: name, Key: key, Type: type, Culture: string.Empty);
    }

    private async Task Teardown(params int[] ids)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        CommonObject[] commonObjects = core
            .Set<CommonObject>()
            .IgnoreQueryFilters()
            .Where(predicate: commonObject => ids.Contains(value: commonObject.Id))
            .ToArray();

        if (commonObjects.Length > 0)
        {
            await core.DeleteAllAsync(commonObjects: commonObjects);
        }
    }

    private async Task<CommonObject> CreateCommonObjectAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<CommonObject>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateCommonObjectAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchCommonObjectAsync(int id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteCommonObjectAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<CommonObject> GetCommonObjectAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");

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

        return JsonSerializer.Deserialize<CommonObject>(json: content, options: JsonOptions);
    }

    private async Task<int> GetCommonObjectCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<CommonObject>> GetCommonObjectsAsync(int top = 1)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<CommonObject>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<IReadOnlyList<CommonObject>> GetLatestCommonObjectsAsync(string type)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/Latest()?type={Uri.EscapeDataString(stringToEscape: type)}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        if (content.Contains(value: "\"value\":", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return JsonSerializer.Deserialize<ODataEnvelope<CommonObject>>(json: content, options: JsonOptions)!.Value;
        }

        return JsonSerializer.Deserialize<List<CommonObject>>(json: content, options: JsonOptions)!;
    }

    private async Task<string> ImportCommonObjectsAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: $"{BaseUrl}/Import", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return content;
    }

    private async Task<IReadOnlyList<CommonObject>> FilterCommonObjectsByKeyAsync(string key)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$filter=Key eq '{key}'");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<CommonObject>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> GetCommonObjectStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}