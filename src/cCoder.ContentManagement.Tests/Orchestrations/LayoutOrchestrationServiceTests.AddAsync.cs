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

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();

        layoutProcessingServiceMock.Setup(expression: x => x.AddLayoutAsync(newLayout: entity))
            .ReturnsAsync(value: entity);

        layoutEventProcessingServiceMock
            .Setup(expression: x => x.RaiseLayoutAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Layout result = await orchestrationService.AddLayoutAsync(newLayout: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        layoutProcessingServiceMock.Verify(expression: x => x.AddLayoutAsync(newLayout: entity), times: Times.Once);
        layoutEventProcessingServiceMock.Verify(expression: x => x.RaiseLayoutAddEventAsync(entity: entity), times: Times.Once);
    }

}