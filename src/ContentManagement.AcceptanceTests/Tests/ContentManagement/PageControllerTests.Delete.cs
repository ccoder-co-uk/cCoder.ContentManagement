using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task Delete_RemovesPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");
        Page createdPage = await CreatePageAsync(CreateValidPagePayload(seededContext, Unique("Page")));
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeletePageAsync(createdPage.Id);
        actualReadStatusCode = await GetPageStatusCodeAsync(createdPage.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





