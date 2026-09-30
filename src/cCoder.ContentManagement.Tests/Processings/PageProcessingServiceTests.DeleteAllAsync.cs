// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachPageWhenUserIsAppAdminForDeleteAllAsync()
    {
        // Given

        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        Page page = CreateRandomPage(user: actor);
        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { page }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.DeleteAsync(pageId: page.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await pageProcessingService.DeleteAllPageAsync(deletedPage: new[] { page });

        // Then
        pageServiceMock.Verify(expression: x => x.DeleteAsync(pageId: page.Id), times: Times.Once);
        VerifyNoOtherPageServiceCalls();
    }
}