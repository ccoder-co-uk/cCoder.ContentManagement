using cCoder.Data.Models;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Import_CreatesCommonObjects()
    {
        // Given
        string name = Unique("ImportedCommonObject");
        string key = Unique("key");

        // When
        await ImportCommonObjectsAsync(new
        {
            value = new object[]
            {
                new
                {
                    name,
                    description = "Imported common object",
                    version = 1,
                    key,
                    type = "Acceptance/Test",
                    json = "{\"enabled\":true}",
                    culture = string.Empty,
                },
            },
        });
        IReadOnlyList<CommonObject> actualCommonObjects = await FilterCommonObjectsByKeyAsync(key);

        // Then
        actualCommonObjects.Select(item => item.Name).Should().Contain(name);
    }
}





