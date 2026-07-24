// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        await PatchCommonObjectAsync(id: seededContext.Id, payload: new
        {
            description = updatedDescription,
        });

        IReadOnlyList<CommonObject> actualCommonObjects = await FilterCommonObjectsByKeyAsync(
key: seededContext.Key
        );

        CommonObject actualCommonObject = actualCommonObjects
            .OrderByDescending(keySelector: item => item.Version)
            .FirstOrDefault(predicate: item => item.Type == seededContext.Type && item.Culture == seededContext.Culture);

        // Then

        actualCommonObject.Should()
            .NotBeNull();

        actualCommonObject!.Description.Should()
            .Be(expected: updatedDescription);

        actualCommonObject.Version.Should()
            .Be(expected: 2);

        foreach (CommonObject commonObject in actualCommonObjects)
        {
            await DeleteCommonObjectAsync(id: commonObject.Id);
        }

        await Teardown(ids: actualCommonObjects.Select(selector: item => item.Id)
            .ToArray());
    }
}