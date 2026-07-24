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
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Package entity = CreateRandomPackage();

        packageProcessingServiceMock.Setup(expression: x => x.UpdatePackageAsync(updatedPackage: entity))
            .ReturnsAsync(value: entity);

        packageEventProcessingServiceMock
            .Setup(expression: x => x.RaisePackageUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Package result = await orchestrationService.UpdatePackageAsync(updatedPackage: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        packageProcessingServiceMock.Verify(expression: x => x.UpdatePackageAsync(updatedPackage: entity), times: Times.Once);
        packageEventProcessingServiceMock.Verify(expression: x => x.RaisePackageUpdateEventAsync(entity: entity), times: Times.Once);
    }

}