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
using System.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteWhenUserCanDeleteAppCultureForDeleteAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();
        appCultureServiceMock
            .Setup(x => x.GetAppCulture(appCulture.AppId, appCulture.CultureId, false))
            .Returns(appCulture);
        appCultureServiceMock.Setup(x => x.DeleteAppCultureAsync(appCulture)).Returns(ValueTask.CompletedTask);

        // When
        await appCultureProcessingService.DeleteAppCultureAsync(appCulture);

        // Then
        appCultureServiceMock.Verify(
            x =>
                x.DeleteAppCultureAsync(
                    It.Is<AppCulture>(item =>
                        item.AppId == appCulture.AppId && item.CultureId == appCulture.CultureId
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenFoundationRejectsDeleteAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();
        appCultureServiceMock
            .Setup(x => x.GetAppCulture(appCulture.AppId, appCulture.CultureId, false))
            .Returns(appCulture);
        appCultureServiceMock
            .Setup(x => x.DeleteAppCultureAsync(appCulture))
            .Throws(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await appCultureProcessingService.DeleteAppCultureAsync(appCulture)
        );

        // Then
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenAppCultureDoesNotExistForDeleteAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();
        appCultureServiceMock
            .Setup(x => x.GetAppCulture(appCulture.AppId, appCulture.CultureId, false))
            .Returns((AppCulture)null);

        // When
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await appCultureProcessingService.DeleteAppCultureAsync(appCulture));

        // Then
    }

}














