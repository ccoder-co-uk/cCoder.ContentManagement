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

public partial class AppCultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        AppCulture entity = CreateRandomAppCulture();

        appCultureProcessingServiceMock.Setup(expression: x => x.AddAppCultureAsync(newAppCulture: entity))
            .ReturnsAsync(value: entity);

        appCultureEventProcessingServiceMock
            .Setup(expression: x => x.RaiseAppCultureAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        AppCulture result = await orchestrationService.AddAppCultureAsync(newAppCulture: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        appCultureProcessingServiceMock.Verify(expression: x => x.AddAppCultureAsync(newAppCulture: entity), times: Times.Once);
        appCultureEventProcessingServiceMock.Verify(expression: x => x.RaiseAppCultureAddEventAsync(entity: entity), times: Times.Once);
    }

}