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

public partial class TemplateOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();

        templateProcessingServiceMock.Setup(expression: x => x.AddTemplateAsync(newTemplate: entity))
            .ReturnsAsync(value: entity);

        templateEventProcessingServiceMock
            .Setup(expression: x => x.RaiseTemplateAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Template result = await orchestrationService.AddTemplateAsync(newTemplate: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        templateProcessingServiceMock.Verify(expression: x => x.AddTemplateAsync(newTemplate: entity), times: Times.Once);
        templateEventProcessingServiceMock.Verify(expression: x => x.RaiseTemplateAddEventAsync(entity: entity), times: Times.Once);
    }

}