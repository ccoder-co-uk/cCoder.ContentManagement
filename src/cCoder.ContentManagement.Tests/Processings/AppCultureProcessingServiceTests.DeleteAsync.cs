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
            .Setup(expression: x => x.GetAppCulture(appId: appCulture.AppId, cultureId: appCulture.CultureId, ignoreFilters: false))
            .Returns(value: appCulture);

        appCultureServiceMock.Setup(expression: x => x.DeleteAppCultureAsync(deletedAppCulture: appCulture))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await appCultureProcessingService.DeleteAppCultureAsync(deletedAppCulture: appCulture);

        // Then

        appCultureServiceMock.Verify(
expression: x =>
                x.DeleteAppCultureAsync(
deletedAppCulture: It.Is<AppCulture>(match: item =>
                        item.AppId == appCulture.AppId && item.CultureId == appCulture.CultureId
                    )
                ),
times: Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenFoundationRejectsDeleteAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        appCultureServiceMock
            .Setup(expression: x => x.GetAppCulture(appId: appCulture.AppId, cultureId: appCulture.CultureId, ignoreFilters: false))
            .Returns(value: appCulture);

        appCultureServiceMock
            .Setup(expression: x => x.DeleteAppCultureAsync(deletedAppCulture: appCulture))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await appCultureProcessingService.DeleteAppCultureAsync(deletedAppCulture: appCulture)
        );

        // Then
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenAppCultureDoesNotExistForDeleteAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        appCultureServiceMock
            .Setup(expression: x => x.GetAppCulture(appId: appCulture.AppId, cultureId: appCulture.CultureId, ignoreFilters: false))
            .Returns(value: (AppCulture)null);

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementDependencyException>(testCode: async () =>
            await appCultureProcessingService.DeleteAppCultureAsync(deletedAppCulture: appCulture));

        // Then
    }

}