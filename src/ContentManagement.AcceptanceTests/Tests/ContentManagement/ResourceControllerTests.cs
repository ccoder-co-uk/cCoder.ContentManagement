// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class ResourceControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/ContentManagement/Resource";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };
    private sealed record ODataEnvelope<T>(List<T> Value);

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private async Task<Resource> CreateResourceAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.Created, because: content);

        return JsonSerializer.Deserialize<Resource>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateResourceAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchResourceAsync(int id, object payload)
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

    private async Task<int> DeleteResourceAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.NoContent, because: content);

        return (int)response.StatusCode;
    }

    private async Task<Resource> GetResourceAsync(int id)
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

        return JsonSerializer.Deserialize<Resource>(json: content, options: JsonOptions);
    }

    private async Task<int> GetResourceCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<Resource>> GetAllResourcesAsync(string resourceKey)
    {
        string filter =
            $"?$filter=AppId eq 1 and Key eq '{resourceKey.Replace(oldValue: "'", newValue: "''")}'";

        using HttpResponseMessage response = await Client.GetAsync(
requestUri: $"{BaseUrl}{filter}"
        );

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<Resource>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> GetResourceStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}