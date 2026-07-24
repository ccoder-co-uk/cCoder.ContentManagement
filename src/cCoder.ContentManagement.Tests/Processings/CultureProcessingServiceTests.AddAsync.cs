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

public partial class CultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        cultureServiceMock.Setup(expression: x => x.AddCultureAsync(newCulture: culture))
            .ReturnsAsync(value: culture);

        // When
        Culture result = await cultureProcessingService.AddCultureAsync(newCulture: culture);

        // Then
        Assert.Same(expected: culture, actual: result);
        cultureServiceMock.Verify(expression: x => x.AddCultureAsync(newCulture: culture), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        cultureServiceMock
            .Setup(expression: x => x.AddCultureAsync(newCulture: culture))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await cultureProcessingService.AddCultureAsync(newCulture: culture)
        );

        // Then
    }

}