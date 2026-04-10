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
    public async Task ShouldDeleteAppWhenUserIsAppAdminForDeleteAsync()
    {
        // Given
        User actor = TestUsers.WithPrivilege("app_admin", 5);

        currentUser = actor;
        appServiceMock.Setup(x => x.DeleteAsync(5)).Returns(ValueTask.CompletedTask);

        // When
        await appProcessingService.DeleteAsync(5);

        // Then
        appServiceMock.Verify(x => x.DeleteAsync(5), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToServiceWhenUserIsNotAppAdminForDeleteAsync()
    {
        // Given
        currentUser = TestUsers.WithoutPrivileges();
        appServiceMock.Setup(x => x.DeleteAsync(5)).Returns(ValueTask.CompletedTask);

        // When
        await appProcessingService.DeleteAsync(5);

        // Then
        appServiceMock.Verify(x => x.DeleteAsync(5), Times.Once);
        appServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}














