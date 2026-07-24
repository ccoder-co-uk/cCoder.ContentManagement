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
    public async Task Put_UpdatesScriptContent()
    {
        // Given
        string scriptName = Unique(prefix: "Script");

        Script createdScript = await CreateScriptAsync(
payload: new
{
    appId = 1,
    name = scriptName,
    description = "Acceptance script",
    key = "Acceptance",
    content = "return 42;",
});

        Script expectedScript = new() { Content = "return 43;" };

        // When

        await UpdateScriptAsync(
id: createdScript.Id,
payload: new
{
    id = createdScript.Id,
    appId = 1,
    name = scriptName,
    description = "Updated acceptance script",
    key = "Acceptance",
    content = "return 43;",
});

        Script actualScript = await GetScriptAsync(id: createdScript.Id);

        // Then

        actualScript.Should()
            .NotBeNull();

        actualScript!.Content.Should()
            .Be(expected: expectedScript.Content);

        await DeleteScriptAsync(id: createdScript.Id);
    }
}