// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
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

        appCultureServiceMock.Setup(expression: x => x.AddAppCultureAsync(newAppCulture: appCulture))
            .ReturnsAsync(value: appCulture);

        // When
        AppCulture result = await appCultureProcessingService.AddAppCultureAsync(newAppCulture: appCulture);

        // Then
        Assert.Same(expected: appCulture, actual: result);
        appCultureServiceMock.Verify(expression: x => x.AddAppCultureAsync(newAppCulture: appCulture), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenFoundationRejectsAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        appCultureServiceMock
            .Setup(expression: x => x.AddAppCultureAsync(newAppCulture: appCulture))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await appCultureProcessingService.AddAppCultureAsync(newAppCulture: appCulture)
        );

        // Then
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenForeignKeyFailsForAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        DbUpdateException exception = new(
message: "FK",
innerException: new Exception(message: "The INSERT statement conflicted with the FOREIGN KEY constraint."));

        appCultureServiceMock
            .Setup(expression: x => x.AddAppCultureAsync(newAppCulture: appCulture))
            .ThrowsAsync(exception: exception);

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementDependencyException>(testCode: async () =>
            await appCultureProcessingService.AddAppCultureAsync(newAppCulture: appCulture));

        // Then
    }

}