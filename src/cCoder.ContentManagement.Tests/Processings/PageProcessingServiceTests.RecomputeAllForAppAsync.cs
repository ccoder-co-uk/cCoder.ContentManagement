// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task ShouldRecomputePathsAndSaveWhenUserIsAppAdminForRecomputeAllForAppAsync()
    {

        // Given



        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        Page page = CreateRandomPage(user: actor);
        page.Name = "Home";
        page.Path = "home";

        page.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        pageServiceMock
            .Setup(expression: x => x.UpdatePageAsync(updatedPage: It.Is<Page>(match: updated => updated.Id == page.Id && updated.Path == string.Empty)))
            .Callback<Page>(action: updated => page.Path = updated.Path)
            .ReturnsAsync(value: page);

        // When
        await pageProcessingService.RecomputeAllForAppAsync(appId: 1);

        // Then
        page.Path.Should()
            .Be(expected: string.Empty);

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.UpdatePageAsync(updatedPage: It.Is<Page>(match: updated => updated.Id == page.Id && updated.Path == string.Empty)), times: Times.Once);
        VerifyNoOtherPageServiceCalls();
    }

}