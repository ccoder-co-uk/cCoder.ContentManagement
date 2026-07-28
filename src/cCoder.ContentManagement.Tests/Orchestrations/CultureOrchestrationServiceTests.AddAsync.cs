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

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureProcessingServiceMock.Setup(expression: x => x.AddCultureAsync(newCulture: entity))
            .ReturnsAsync(value: entity);

        cultureEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCultureAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Culture result = await orchestrationService.AddCultureAsync(newCulture: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        cultureProcessingServiceMock.Verify(expression: x => x.AddCultureAsync(newCulture: entity), times: Times.Once);
        cultureEventProcessingServiceMock.Verify(expression: x => x.RaiseCultureAddEventAsync(entity: entity), times: Times.Once);
    }

}