// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenRoot_ShouldRedirectToManualTestingShell()
    {
        // Given
        // When
        using HttpResponseMessage response = await Client.GetAsync(requestUri: "/");

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.Redirect);

        response.Headers.Location?.OriginalString.Should()
            .Be(expected: "/tools/index.html");
    }
}