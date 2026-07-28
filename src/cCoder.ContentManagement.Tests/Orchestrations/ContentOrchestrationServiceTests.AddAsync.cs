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

public partial class ContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Content entity = CreateRandomContent();

        contentProcessingServiceMock.Setup(expression: x => x.AddContentAsync(newContent: entity))
            .ReturnsAsync(value: entity);

        contentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseContentAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Content result = await orchestrationService.AddContentAsync(newContent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        contentProcessingServiceMock.Verify(expression: x => x.AddContentAsync(newContent: entity), times: Times.Once);
        contentEventProcessingServiceMock.Verify(expression: x => x.RaiseContentAddEventAsync(entity: entity), times: Times.Once);
    }

}