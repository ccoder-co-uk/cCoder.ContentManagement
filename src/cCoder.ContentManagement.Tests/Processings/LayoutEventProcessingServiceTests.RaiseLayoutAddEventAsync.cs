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

public partial class LayoutEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseLayoutAddEventAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();

        layoutEventServiceMock
            .Setup(expression: x => x.RaiseLayoutAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseLayoutAddEventAsync(entity: entity);

        // Then
        layoutEventServiceMock.Verify(expression: x => x.RaiseLayoutAddEventAsync(entity: entity), times: Times.Once);
        layoutEventServiceMock.VerifyNoOtherCalls();
    }

}