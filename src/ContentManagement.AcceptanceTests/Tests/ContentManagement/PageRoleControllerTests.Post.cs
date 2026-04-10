using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageRoleControllerTests
{
    [Fact]
    public async Task Post_CreatesPageRole()
    {
        // Given
        SeededPageRoleContext seededContext = await SeedDatabase(false, "app_admin", "page_read", "pagerole_create", "pagerole_delete");
        PageRole actualPageRole;

        // When
        actualPageRole = await CreatePageRoleAsync(new
        {
            pageId = seededContext.PageId,
            roleId = seededContext.RoleId,
        });

        // Then
        actualPageRole.Should().NotBeNull();
        actualPageRole.PageId.Should().Be(seededContext.PageId);
        actualPageRole.RoleId.Should().Be(seededContext.RoleId);

        await Teardown(seededContext);
    }
}





