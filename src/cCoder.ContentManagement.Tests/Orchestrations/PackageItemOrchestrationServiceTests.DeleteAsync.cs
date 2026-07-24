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

public partial class PackageItemOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        Guid id = Guid.NewGuid();
        PackageItem entity = CreateRandomPackageItem();

        packageItemProcessingServiceMock.Setup(expression: x => x.GetPackageItem(packageItemId: id))
            .Returns(value: entity);

        packageItemProcessingServiceMock.Setup(expression: x => x.DeleteAsync(packageItemId: id))
            .Returns(value: ValueTask.CompletedTask);

        packageItemEventProcessingServiceMock
            .Setup(expression: x => x.RaisePackageItemDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.DeleteAsync(packageItemId: id);

        packageItemProcessingServiceMock.Verify(expression: x => x.GetPackageItem(packageItemId: id), times: Times.Once);
        packageItemProcessingServiceMock.Verify(expression: x => x.DeleteAsync(packageItemId: id), times: Times.Once);
        packageItemProcessingServiceMock.VerifyNoOtherCalls();
        packageItemEventProcessingServiceMock.Verify(expression: x => x.RaisePackageItemDeleteEventAsync(entity: entity), times: Times.Once);
        packageItemEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}