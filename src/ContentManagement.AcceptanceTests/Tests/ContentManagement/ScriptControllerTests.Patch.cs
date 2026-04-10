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
            new
            {
                appId = 1,
                name = Unique("Script"),
                description = "Acceptance script",
                key = "Acceptance",
                content = "return 42;",
            });
        Script expectedScript = new() { Content = "return 44;" };

        // When
        await PatchScriptAsync(createdScript.Id, new { content = "return 44;" });
        Script actualScript = await GetScriptAsync(createdScript.Id);

        // Then
        actualScript.Should().NotBeNull();
        actualScript!.Content.Should().Be(expectedScript.Content);

        await DeleteScriptAsync(createdScript.Id);
    }
}






