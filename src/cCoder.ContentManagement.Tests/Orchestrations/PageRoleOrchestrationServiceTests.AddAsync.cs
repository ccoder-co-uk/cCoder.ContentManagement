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

public partial class PageRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        PageRole entity = CreateRandomPageRole();

        pageRoleProcessingServiceMock.Setup(expression: x => x.AddPageRoleAsync(newPageRole: entity))
            .ReturnsAsync(value: entity);

        pageRoleEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageRoleAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        PageRole result = await orchestrationService.AddPageRoleAsync(newPageRole: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageRoleProcessingServiceMock.Verify(expression: x => x.AddPageRoleAsync(newPageRole: entity), times: Times.Once);
        pageRoleEventProcessingServiceMock.Verify(expression: x => x.RaisePageRoleAddEventAsync(entity: entity), times: Times.Once);
    }

}