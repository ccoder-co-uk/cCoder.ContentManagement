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

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        CommonObject entity = CreateRandomCommonObject();

        commonObjectProcessingServiceMock.Setup(expression: x => x.GetCommonObject(commonObjectId: id))
            .Returns(value: entity);

        commonObjectProcessingServiceMock.Setup(expression: x => x.DeleteAsync(commonObjectId: id))
            .Returns(value: ValueTask.CompletedTask);

        commonObjectEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCommonObjectDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(commonObjectId: id);

        // Then
        commonObjectProcessingServiceMock.Verify(expression: x => x.GetCommonObject(commonObjectId: id), times: Times.Once);
        commonObjectProcessingServiceMock.Verify(expression: x => x.DeleteAsync(commonObjectId: id), times: Times.Once);
        commonObjectEventProcessingServiceMock.Verify(expression: x => x.RaiseCommonObjectDeleteEventAsync(entity: entity), times: Times.Once);
    }

}