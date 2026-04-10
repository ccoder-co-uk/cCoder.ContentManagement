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

public partial class AppProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachAppWhenUserIsAppAdminForDeleteAllAsync()
    {
        // Given
        User admin = TestUsers.WithPrivilege("app_admin", 1);
        App app = CreateRandomApp();
        app.Id = 1;
        currentUser = admin;
        appServiceMock.Setup(x => x.DeleteAsync(app.Id)).Returns(ValueTask.CompletedTask);

        // When
        await appProcessingService.DeleteAllAsync(new[] { app });

        // Then
        appServiceMock.Verify(x => x.DeleteAsync(app.Id), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















