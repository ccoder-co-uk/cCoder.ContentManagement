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

public partial class TemplateEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseTemplateDeleteEventAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();

        templateEventServiceMock
            .Setup(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseTemplateDeleteEventAsync(entity: entity);

        // Then
        templateEventServiceMock.Verify(expression: x => x.RaiseTemplateDeleteEventAsync(entity: entity), times: Times.Once);
        templateEventServiceMock.VerifyNoOtherCalls();
    }

}