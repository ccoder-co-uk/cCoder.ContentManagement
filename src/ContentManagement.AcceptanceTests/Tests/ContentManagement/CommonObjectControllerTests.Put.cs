using cCoder.Data.Models;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Put_UpdatesCommonObject()
    {
        // Given
        SeededCommonObjectContext seededContext = await SeedDatabase();
        string updatedDescription = "Updated common object";
        string updatedJson = "{\"enabled\":false}";

        // When
        await UpdateCommonObjectAsync(seededContext.Id, new
        {
            id = seededContext.Id,
            name = seededContext.Name,
            description = updatedDescription,
            version = 2,
            key = seededContext.Key,
            type = seededContext.Type,
            json = updatedJson,
            culture = seededContext.Culture,
        });

        IReadOnlyList<CommonObject> actualCommonObjects = await FilterCommonObjectsByKeyAsync(
            seededContext.Key
        );
        CommonObject actualCommonObject = actualCommonObjects
            .OrderByDescending(item => item.Version)
            .FirstOrDefault(item => item.Type == seededContext.Type && item.Culture == seededContext.Culture);

        // Then
        actualCommonObject.Should().NotBeNull();
        actualCommonObject!.Description.Should().Be(updatedDescription);
        actualCommonObject.Json.Should().Be(updatedJson);
        actualCommonObject.Version.Should().Be(2);

        foreach (CommonObject commonObject in actualCommonObjects)
            await DeleteCommonObjectAsync(commonObject.Id);

        await Teardown(actualCommonObjects.Select(item => item.Id).ToArray());
    }
}






