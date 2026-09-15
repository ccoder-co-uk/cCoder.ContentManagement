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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseResourceAddEventAsync()
    {
        // Given
        Resource entity = CreateRandomResource();

        resourceEventServiceMock
            .Setup(expression: x => x.RaiseResourceAddEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseResourceAddEventAsync(
            resource: entity,
            userId: CurrentUserId);

        // Then
        resourceEventServiceMock.Verify(expression: x => x.RaiseResourceAddEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        resourceEventServiceMock.VerifyNoOtherCalls();
    }

}