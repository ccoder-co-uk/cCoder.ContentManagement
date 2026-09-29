// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
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