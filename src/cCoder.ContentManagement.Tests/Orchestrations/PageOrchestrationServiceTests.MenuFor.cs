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

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenMenuFor()
    {
        // Given
        pageProcessingServiceMock.Setup(expression: x => x.MenuFor(pageId: 1, culture: "en-GB"))
            .Returns(value: "menu");

        // When
        string result = orchestrationService.MenuFor(pageId: 1, culture: "en-GB");

        // Then
        result.Should()
            .Be(expected: "menu");

        pageProcessingServiceMock.Verify(expression: x => x.MenuFor(pageId: 1, culture: "en-GB"), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}