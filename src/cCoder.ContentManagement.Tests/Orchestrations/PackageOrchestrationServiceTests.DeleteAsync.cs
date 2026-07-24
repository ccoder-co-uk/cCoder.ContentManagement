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



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        Package entity = CreateRandomPackage();

        packageProcessingServiceMock.Setup(expression: x => x.GetPackage(packageId: id))
            .Returns(value: entity);

        packageProcessingServiceMock.Setup(expression: x => x.DeleteAsync(packageId: id))
            .Returns(value: ValueTask.CompletedTask);

        packageEventProcessingServiceMock
            .Setup(expression: x => x.RaisePackageDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(packageId: id);

        // Then
        packageProcessingServiceMock.Verify(expression: x => x.GetPackage(packageId: id), times: Times.Once);
        packageProcessingServiceMock.Verify(expression: x => x.DeleteAsync(packageId: id), times: Times.Once);
        packageEventProcessingServiceMock.Verify(expression: x => x.RaisePackageDeleteEventAsync(entity: entity), times: Times.Once);
    }

}