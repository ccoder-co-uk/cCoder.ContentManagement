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

        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
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

        await Assert.ThrowsAsync<InvalidOperationException>(testCode: async () =>
            await appCultureProcessingService.AddAppCultureAsync(newAppCulture: appCulture));

        // Then
    }

}