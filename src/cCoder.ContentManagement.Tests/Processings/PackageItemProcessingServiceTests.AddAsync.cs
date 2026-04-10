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

public partial class PackageItemProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();
        packageItemServiceMock.Setup(x => x.AddAsync(packageItem)).ReturnsAsync(packageItem);

        // When
        PackageItem result = await packageItemProcessingService.AddAsync(packageItem);

        // Then
        Assert.Same(packageItem, result);
        packageItemServiceMock.Verify(x => x.AddAsync(packageItem), Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        PackageItem packageItem = CreateRandomPackageItem();

        packageItemServiceMock
            .Setup(x => x.AddAsync(packageItem))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await packageItemProcessingService.AddAsync(packageItem)
        );

        // Then
    }

}















