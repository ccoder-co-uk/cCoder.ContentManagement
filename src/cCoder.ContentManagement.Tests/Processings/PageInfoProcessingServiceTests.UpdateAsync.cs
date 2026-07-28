// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUserCanUpdatePageInfoForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        pageInfoServiceMock.Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: pageInfo))
            .ReturnsAsync(value: pageInfo);

        // When
        PageInfo result = await pageInfoProcessingService.UpdatePageInfoAsync(updatedPageInfo: pageInfo);

        // Then
        Assert.Same(expected: pageInfo, actual: result);
        pageInfoServiceMock.Verify(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: pageInfo), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        pageInfoServiceMock
            .Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: pageInfo))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await pageInfoProcessingService.UpdatePageInfoAsync(updatedPageInfo: pageInfo)
        );

        // Then
    }

}