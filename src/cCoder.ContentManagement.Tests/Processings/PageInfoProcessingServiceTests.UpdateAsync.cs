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

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUserCanUpdatePageInfoForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();
        pageInfoServiceMock.Setup(x => x.UpdateAsync(pageInfo)).ReturnsAsync(pageInfo);

        // When
        PageInfo result = await pageInfoProcessingService.UpdateAsync(pageInfo);

        // Then
        Assert.Same(pageInfo, result);
        pageInfoServiceMock.Verify(x => x.UpdateAsync(pageInfo), Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        pageInfoServiceMock
            .Setup(x => x.UpdateAsync(pageInfo))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await pageInfoProcessingService.UpdateAsync(pageInfo)
        );

        // Then
    }

}















