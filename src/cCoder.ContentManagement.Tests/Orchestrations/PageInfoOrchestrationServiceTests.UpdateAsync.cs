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

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();

        pageInfoProcessingServiceMock.Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: entity))
            .ReturnsAsync(value: entity);

        pageInfoEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageInfoUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageInfo result = await orchestrationService.UpdatePageInfoAsync(updatedPageInfo: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageInfoProcessingServiceMock.Verify(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: entity), times: Times.Once);
        pageInfoEventProcessingServiceMock.Verify(expression: x => x.RaisePageInfoUpdateEventAsync(entity: entity), times: Times.Once);
    }

}