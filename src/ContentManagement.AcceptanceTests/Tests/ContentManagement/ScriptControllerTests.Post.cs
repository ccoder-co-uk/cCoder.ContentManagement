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
    public async Task Post_CreatesScript()
    {
        // Given
        string scriptName = Unique(prefix: "Script");
        Script expectedScript = new() { Name = scriptName };

        // When

        Script createdScript = await CreateScriptAsync(
payload: new
{
    appId = 1,
    name = scriptName,
    description = "Acceptance script",
    key = "Acceptance",
    content = "return 42;",
});

        Script actualScript = await GetScriptAsync(id: createdScript.Id);

        // Then

        actualScript.Should()
            .NotBeNull();

        actualScript!.Name.Should()
            .Be(expected: expectedScript.Name);

        await DeleteScriptAsync(id: createdScript.Id);
    }
}