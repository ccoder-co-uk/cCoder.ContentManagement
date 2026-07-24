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
    public async Task Post_CreatesCulture()
    {
        // Given
        string cultureId = Unique(prefix: "culture");
        Culture expectedCulture = new() { Id = cultureId };

        // When

        await CreateCultureAsync(payload: new
        {
            id = cultureId,
            name = Unique(prefix: "Culture"),
        });

        Culture actualCulture = await GetCultureAsync(id: cultureId);

        // Then

        actualCulture.Should()
            .NotBeNull();

        actualCulture!.Id.Should()
            .Be(expected: expectedCulture.Id);

        await DeleteCultureAsync(id: cultureId);
        await Teardown(cultureIds: cultureId);
    }
}