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
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageProcessingServiceTests
{
    [Fact]
    public async Task ShouldPersistAsGraphViaFoundationServiceWhenPackageHasItemsForAddAsync()
    {
        // Given
        Package package = CreateRandomPackage();
        package.Items = [CreateRandomPackageItem(), CreateRandomPackageItem()];

        packageServiceMock.Setup(x => x.AddPackageAsync(package)).ReturnsAsync(package);

        // When
        Package result = await packageProcessingService.AddPackageAsync(package);

        // Then
        Assert.Same(package, result);
        packageServiceMock.Verify(x => x.AddPackageAsync(package), Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }

}













