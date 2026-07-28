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
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageItemOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        PackageItem entity = CreateRandomPackageItem();

        packageItemProcessingServiceMock.Setup(expression: x => x.UpdatePackageItemAsync(updatedPackageItem: entity))
            .ReturnsAsync(value: entity);

        packageItemEventProcessingServiceMock
            .Setup(expression: x => x.RaisePackageItemUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PackageItem result = await orchestrationService.UpdatePackageItemAsync(updatedPackageItem: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        packageItemProcessingServiceMock.Verify(expression: x => x.UpdatePackageItemAsync(updatedPackageItem: entity), times: Times.Once);
        packageItemProcessingServiceMock.VerifyNoOtherCalls();
        packageItemEventProcessingServiceMock.Verify(expression: x => x.RaisePackageItemUpdateEventAsync(entity: entity), times: Times.Once);
        packageItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}