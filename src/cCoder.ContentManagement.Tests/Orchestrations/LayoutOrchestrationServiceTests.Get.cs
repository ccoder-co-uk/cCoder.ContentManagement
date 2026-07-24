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

public partial class LayoutOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        Layout entity = CreateRandomLayout();

        layoutProcessingServiceMock.Setup(expression: x => x.GetLayout(layoutId: id))
            .Returns(value: entity);

        // When
        Layout result = orchestrationService.GetLayout(layoutId: id);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: entity);

        layoutProcessingServiceMock.Verify(expression: x => x.GetLayout(layoutId: id), times: Times.Once);
        layoutProcessingServiceMock.VerifyNoOtherCalls();
        layoutEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}