// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CultureControllerTests
{
    [Fact]
    public async Task Patch_UpdatesCulture()
    {
        // Given
        SeededCultureContext seededContext = await SeedDatabase();
        string updatedName = Unique(prefix: "PatchedCulture");
        Culture expectedCulture = new() { Name = updatedName };

        // When

        await PatchCultureAsync(id: seededContext.CultureId, payload: new
        {
            name = updatedName,
        });

        Culture actualCulture = await GetCultureAsync(id: seededContext.CultureId);

        // Then

        actualCulture.Should()
            .NotBeNull();

        actualCulture!.Name.Should()
            .Be(expected: expectedCulture.Name);

        await DeleteCultureAsync(id: seededContext.CultureId);
        await Teardown(cultureIds: seededContext.CultureId);
    }
}