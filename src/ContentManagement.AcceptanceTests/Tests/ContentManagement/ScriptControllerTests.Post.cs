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
        string scriptName = Unique("Script");
        Script expectedScript = new() { Name = scriptName };

        // When
        Script createdScript = await CreateScriptAsync(
            new
            {
                appId = 1,
                name = scriptName,
                description = "Acceptance script",
                key = "Acceptance",
                content = "return 42;",
            });
        Script actualScript = await GetScriptAsync(createdScript.Id);

        // Then
        actualScript.Should().NotBeNull();
        actualScript!.Name.Should().Be(expectedScript.Name);

        await DeleteScriptAsync(createdScript.Id);
    }
}






