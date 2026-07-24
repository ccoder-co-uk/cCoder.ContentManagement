// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class ScriptControllerTests
{
    [Fact]
    public async Task Delete_RemovesScript()
    {
        // Given
        Script createdScript = await CreateScriptAsync(
payload: new
{
    appId = 1,
    name = Unique(prefix: "Script"),
    description = "Acceptance script",
    key = "Acceptance",
    content = "return 42;",
});

        // When
        int actualStatusCode = await DeleteScriptAsync(id: createdScript.Id);
        int actualReadStatusCode = await GetScriptStatusCodeAsync(id: createdScript.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);
    }
}