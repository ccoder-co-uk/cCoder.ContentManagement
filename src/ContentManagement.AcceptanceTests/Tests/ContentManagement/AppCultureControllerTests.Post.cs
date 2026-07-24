// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppCultureControllerTests
{
    [Fact]
    public async Task Post_CreatesAppCulture()
    {
        // Given
        SeededAppCultureContext seededContext = await SeedDatabase(includeAppCulture: false, "appculture_create", "appculture_delete");
        AppCulture actualAppCulture;

        // When

        await CreateAppCultureAsync(payload: new
        {
            appId = seededContext.AppId,
            cultureId = seededContext.CultureId,
        });

        actualAppCulture = await FindAppCultureAsync(appId: seededContext.AppId, cultureId: seededContext.CultureId);

        // Then

        actualAppCulture.Should()
            .NotBeNull();

        actualAppCulture!.AppId.Should()
            .Be(expected: seededContext.AppId);

        actualAppCulture.CultureId.Should()
            .Be(expected: seededContext.CultureId);

        await Teardown(seededContext: seededContext);
    }
}