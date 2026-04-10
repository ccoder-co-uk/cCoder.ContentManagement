using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageControllerTests
{
    [Fact]
    public async Task RootFor_ReturnsRootPage()
    {
        // Given
        SeededPageContext seededContext = await SeedDatabase("page_create", "page_delete");
        string rootName = Unique("RootPage");
        int rootId = (await CreatePageAsync(
            CreateValidPagePayload(seededContext, rootName)
        )).Id;

        string childName = Unique("ChildPage");
        int childId = (await CreatePageAsync(
            CreateValidPagePayload(seededContext, childName, 2, true, "Default", rootId)
        )).Id;

        // When
        Page actualRootPage = await GetRootPageAsync(childId);

        // Then
        actualRootPage.Should().NotBeNull();
        actualRootPage!.Id.Should().Be(rootId);

        await DeletePageAsync(childId);
        await DeletePageAsync(rootId);
        await Teardown(seededContext);
    }
}






