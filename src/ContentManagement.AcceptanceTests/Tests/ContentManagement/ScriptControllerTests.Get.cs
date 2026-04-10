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
            new
            {
                appId = 1,
                name = Unique("Script"),
                description = "Acceptance script",
                key = "Acceptance",
                content = "return 42;",
            });
        Script expectedScript = new() { Id = createdScript.Id };

        // When
        Script actualScript = await GetScriptAsync(createdScript.Id);

        // Then
        actualScript.Should().NotBeNull();
        actualScript!.Id.Should().Be(expectedScript.Id);

        await DeleteScriptAsync(createdScript.Id);
    }

    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetScriptCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }
}






