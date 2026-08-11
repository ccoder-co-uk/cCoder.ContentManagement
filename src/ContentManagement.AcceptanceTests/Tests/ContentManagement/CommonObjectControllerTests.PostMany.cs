// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using cCoder.Data.Models;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Post_ArrayCreatesEveryCommonObjectThroughNormalEndpoint()
    {
        // Given
        string firstName = Unique(prefix: "PostedCommonObject");
        string secondName = Unique(prefix: "PostedCommonObject");
        string firstKey = Unique(prefix: "key");
        string secondKey = Unique(prefix: "key");

        object[] payload =
        [
            new
            {
                name = firstName,
                description = "First posted common object",
                version = 1,
                key = firstKey,
                type = "Acceptance/Test",
                json = "{\"enabled\":true}",
                culture = string.Empty,
            },
            new
            {
                name = secondName,
                description = "Second posted common object",
                version = 1,
                key = secondKey,
                type = "Acceptance/Test",
                json = "{\"enabled\":true}",
                culture = string.Empty,
            },
        ];

        // When
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
            requestUri: BaseUrl,
            value: payload);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.Created);

        IReadOnlyList<CommonObject> firstObjects =
            await FilterCommonObjectsByKeyAsync(key: firstKey);

        IReadOnlyList<CommonObject> secondObjects =
            await FilterCommonObjectsByKeyAsync(key: secondKey);

        firstObjects.Should()
            .ContainSingle(predicate: item => item.Name == firstName);

        secondObjects.Should()
            .ContainSingle(predicate: item => item.Name == secondName);

        await Teardown(ids:
        [
            .. firstObjects.Select(selector: item => item.Id),
            .. secondObjects.Select(selector: item => item.Id),
        ]);
    }
}