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
        string scriptName = Unique("Script");
        Script createdScript = await CreateScriptAsync(
            new
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
            createdScript.Id,
            new
            {
                id = createdScript.Id,
                appId = 1,
                name = scriptName,
                description = "Updated acceptance script",
                key = "Acceptance",
                content = "return 43;",
            });
        Script actualScript = await GetScriptAsync(createdScript.Id);

        // Then
        actualScript.Should().NotBeNull();
        actualScript!.Content.Should().Be(expectedScript.Content);

        await DeleteScriptAsync(createdScript.Id);
    }
}






