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
using Microsoft.EntityFrameworkCore;
using System.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseDataContextWhenUserCanCreateAppCultureForAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();
        appCultureServiceMock.Setup(x => x.AddAsync(appCulture)).ReturnsAsync(appCulture);

        // When
        AppCulture result = await appCultureProcessingService.AddAsync(appCulture);

        // Then
        Assert.Same(appCulture, result);
        appCultureServiceMock.Verify(x => x.AddAsync(appCulture), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenFoundationRejectsAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();
        appCultureServiceMock
            .Setup(x => x.AddAsync(appCulture))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await appCultureProcessingService.AddAsync(appCulture)
        );

        // Then
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenForeignKeyFailsForAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();
        DbUpdateException exception = new(
            "FK",
            new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint."));

        appCultureServiceMock
            .Setup(x => x.AddAsync(appCulture))
            .ThrowsAsync(exception);

        // When
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await appCultureProcessingService.AddAsync(appCulture));

        // Then
    }

}














