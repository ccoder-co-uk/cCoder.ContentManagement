using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageInfoControllerTests
{
    [Fact]
    public async Task Delete_RemovesPageInfo()
    {
        // Given
        SeededPageInfoContext seededContext = await SeedDatabase(includePageInfo: true);
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeletePageInfoAsync(seededContext.PageInfoId);
        actualReadStatusCode = await GetPageInfoStatusCodeAsync(seededContext.PageInfoId);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





