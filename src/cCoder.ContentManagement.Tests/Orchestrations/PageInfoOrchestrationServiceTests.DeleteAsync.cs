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

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        PageInfo entity = CreateRandomPageInfo();

        pageInfoProcessingServiceMock.Setup(expression: x => x.GetPageInfo(pageInfoId: id))
            .Returns(value: entity);

        pageInfoProcessingServiceMock.Setup(expression: x => x.DeleteAsync(pageInfoId: id))
            .Returns(value: ValueTask.CompletedTask);

        pageInfoEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageInfoDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(pageInfoId: id);

        // Then
        pageInfoProcessingServiceMock.Verify(expression: x => x.GetPageInfo(pageInfoId: id), times: Times.Once);
        pageInfoProcessingServiceMock.Verify(expression: x => x.DeleteAsync(pageInfoId: id), times: Times.Once);
        pageInfoEventProcessingServiceMock.Verify(expression: x => x.RaisePageInfoDeleteEventAsync(entity: entity), times: Times.Once);
    }

}