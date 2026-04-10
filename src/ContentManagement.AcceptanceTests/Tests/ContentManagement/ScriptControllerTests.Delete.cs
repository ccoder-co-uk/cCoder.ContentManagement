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
            new
            {
                appId = 1,
                name = Unique("Script"),
                description = "Acceptance script",
                key = "Acceptance",
                content = "return 42;",
            });

        // When
        int actualStatusCode = await DeleteScriptAsync(createdScript.Id);
        int actualReadStatusCode = await GetScriptStatusCodeAsync(createdScript.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);
    }
}





