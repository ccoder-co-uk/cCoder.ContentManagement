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

public partial class CultureEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseCultureAddEventAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();

        cultureEventServiceMock
            .Setup(expression: x => x.RaiseCultureAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCultureAddEventAsync(entity: entity);

        // Then
        cultureEventServiceMock.Verify(expression: x => x.RaiseCultureAddEventAsync(entity: entity), times: Times.Once);
        cultureEventServiceMock.VerifyNoOtherCalls();
    }

}