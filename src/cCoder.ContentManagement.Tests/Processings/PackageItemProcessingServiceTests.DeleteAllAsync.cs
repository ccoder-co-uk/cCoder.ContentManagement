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
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageItemProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        PackageItem entity = CreateRandomPackageItem();
        var id = entity.Id;

        packageItemServiceMock.Setup(expression: x => x.DeleteAsync(packageItemId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await packageItemProcessingService.DeleteAllPackageItemAsync(deletedPackageItem: new[] { entity });

        // Then
        packageItemServiceMock.Verify(expression: x => x.DeleteAsync(packageItemId: id), times: Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }

}