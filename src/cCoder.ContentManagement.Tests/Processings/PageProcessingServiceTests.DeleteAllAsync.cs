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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);
        User actor = TestUsers.WithPrivilege("app_admin", 1);
        Page page = CreateRandomPage(actor);
        currentUser = actor;
        pageServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { page }.AsQueryable());
        pageServiceMock.Setup(x => x.DeleteAsync(page.Id)).Returns(ValueTask.CompletedTask);

        await pageProcessingService.DeleteAllAsync(new[] { page });

        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.Verify(x => x.DeleteAsync(page.Id), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}











