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



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageItemOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        PackageItem[] entities = [CreateRandomPackageItem()];

        packageItemProcessingServiceMock.Setup(expression: x => x.DeleteAllPackageItemAsync(deletedPackageItem: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllPackageItemAsync(deletedPackageItem: entities);

        // Then
        packageItemProcessingServiceMock.Verify(expression: x => x.DeleteAllPackageItemAsync(deletedPackageItem: entities), times: Times.Once);
        packageItemProcessingServiceMock.VerifyNoOtherCalls();
        packageItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}