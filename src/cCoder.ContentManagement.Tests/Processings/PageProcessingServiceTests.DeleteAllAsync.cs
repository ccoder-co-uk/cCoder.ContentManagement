// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachPageWhenUserIsAppAdminForDeleteAllAsync()
    {
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        Page page = CreateRandomPage(user: actor);
        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: new[] { page }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.DeleteAsync(pageId: page.Id))
            .Returns(value: ValueTask.CompletedTask);

        await pageProcessingService.DeleteAllPageAsync(deletedPage: new[] { page });

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.DeleteAsync(pageId: page.Id), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}