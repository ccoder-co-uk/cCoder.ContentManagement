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

public partial class ComponentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Component entity = CreateRandomComponent();

        componentProcessingServiceMock.Setup(expression: x => x.UpdateComponentAsync(updatedComponent: entity))
            .ReturnsAsync(value: entity);

        componentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseComponentUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Component result = await orchestrationService.UpdateComponentAsync(updatedComponent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        componentProcessingServiceMock.Verify(expression: x => x.UpdateComponentAsync(updatedComponent: entity), times: Times.Once);
        componentEventProcessingServiceMock.Verify(expression: x => x.RaiseComponentUpdateEventAsync(entity: entity), times: Times.Once);
    }

}