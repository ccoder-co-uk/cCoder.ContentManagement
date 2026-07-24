// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenHealthEndpoint_ShouldReturnOk()
    {
        // Given
        // When
        string content = await GetOkContentAsync(path: "/Health");

        // Then
        content.Should()
            .Be(expected: "OK");
    }
}