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
    public async Task Get_ReturnsCreatedScript()
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

        Script expectedScript = new() { Id = createdScript.Id };

        // When
        Script actualScript = await GetScriptAsync(id: createdScript.Id);

        // Then

        actualScript.Should()
            .NotBeNull();

        actualScript!.Id.Should()
            .Be(expected: expectedScript.Id);

        await DeleteScriptAsync(id: createdScript.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetScriptCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }
}