// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Users_WhenRoleContainsUser_ShouldReturnUser()
    {
        // Given
        SeededApp seededApp = await SeedDatabase(privileges: ["app_read"]);

        // When
        using HttpResponseMessage response = await Client.GetAsync(
            requestUri: $"{BaseUrl}({seededApp.AppId})/Users()");

        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        ODataEnvelope<User> users = JsonSerializer.Deserialize<ODataEnvelope<User>>(
            json: content,
            options: JsonOptions);

        users.Value.Should()
            .ContainSingle(predicate: user => user.Id == "Guest");

        await Teardown(seededApp: seededApp);
    }
}