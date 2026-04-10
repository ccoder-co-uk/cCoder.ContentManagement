using cCoder.Data.Models;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Patch_UpdatesCommonObject()
    {
        // Given
        SeededCommonObjectContext seededContext = await SeedDatabase();
        string updatedDescription = "Patched common object";

        // When
        await PatchCommonObjectAsync(seededContext.Id, new
        {
            description = updatedDescription,
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
        actualCommonObject.Version.Should().Be(2);

        foreach (CommonObject commonObject in actualCommonObjects)
            await DeleteCommonObjectAsync(commonObject.Id);

        await Teardown(actualCommonObjects.Select(item => item.Id).ToArray());
    }
}






