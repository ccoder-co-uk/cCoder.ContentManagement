// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class PageRoleControllerTests
{
    [Fact]
    public async Task Delete_RemovesPageRole()
    {
        // Given
        SeededPageRoleContext seededContext = await SeedDatabase(
            includePageRole: true,
            privileges: ["app_admin", "page_read", "pagerole_delete"]);

        // When
        await DeletePageRoleAsync(
            pageId: seededContext.PageId,
            roleId: seededContext.RoleId);

        // Then
        (await FindPageRoleAsync(
            pageId: seededContext.PageId,
            roleId: seededContext.RoleId)).Should()
            .BeNull();

        await Teardown(seededContext: seededContext);
    }
}