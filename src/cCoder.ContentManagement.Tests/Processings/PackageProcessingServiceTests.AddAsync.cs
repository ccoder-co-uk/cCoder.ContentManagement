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

        packageServiceMock.Setup(expression: x => x.AddPackageAsync(newPackage: package))
            .ReturnsAsync(value: package);

        // When
        Package result = await packageProcessingService.AddPackageAsync(newPackage: package);

        // Then
        Assert.Same(expected: package, actual: result);
        packageServiceMock.Verify(expression: x => x.AddPackageAsync(newPackage: package), times: Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }

}