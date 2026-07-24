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
    public async Task Patch_UpdatesScriptContent()
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

        Script expectedScript = new() { Content = "return 44;" };

        // When
        await PatchScriptAsync(id: createdScript.Id, payload: new { content = "return 44;" });
        Script actualScript = await GetScriptAsync(id: createdScript.Id);

        // Then

        actualScript.Should()
            .NotBeNull();

        actualScript!.Content.Should()
            .Be(expected: expectedScript.Content);

        await DeleteScriptAsync(id: createdScript.Id);
    }
}