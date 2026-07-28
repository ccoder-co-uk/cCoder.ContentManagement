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

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();

        commonObjectProcessingServiceMock.Setup(expression: x => x.UpdateCommonObjectAsync(updatedCommonObject: entity))
            .ReturnsAsync(value: entity);

        commonObjectEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCommonObjectUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        CommonObject result = await orchestrationService.UpdateCommonObjectAsync(updatedCommonObject: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        commonObjectProcessingServiceMock.Verify(expression: x => x.UpdateCommonObjectAsync(updatedCommonObject: entity), times: Times.Once);
        commonObjectEventProcessingServiceMock.Verify(expression: x => x.RaiseCommonObjectUpdateEventAsync(entity: entity), times: Times.Once);
    }

}