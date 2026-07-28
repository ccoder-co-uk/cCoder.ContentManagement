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

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenResolveCurrentApp()
    {
        // Given
        App app = CreateRandomApp();

        appProcessingServiceMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        // When
        App result = orchestrationService.ResolveCurrentApp();

        // Then
        result.Should()
            .BeEquivalentTo(expectation: app, config: options => options.Excluding(expression: entity => entity.Config));

        appProcessingServiceMock.Verify(expression: x => x.ResolveCurrentApp(), times: Times.Once);
        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}