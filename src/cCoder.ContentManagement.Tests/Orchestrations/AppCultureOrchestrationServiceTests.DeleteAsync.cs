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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppCultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        appCultureProcessingServiceMock.Setup(expression: x => x.DeleteAppCultureAsync(deletedAppCulture: appCulture))
            .Returns(value: ValueTask.CompletedTask);

        appCultureEventProcessingServiceMock
            .Setup(expression: x => x.RaiseAppCultureDeleteEventAsync(entity: appCulture))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAppCultureAsync(deletedAppCulture: appCulture);

        // Then
        appCultureProcessingServiceMock.Verify(expression: x => x.DeleteAppCultureAsync(deletedAppCulture: appCulture), times: Times.Once);
        appCultureEventProcessingServiceMock.Verify(expression: x => x.RaiseAppCultureDeleteEventAsync(entity: appCulture), times: Times.Once);
    }

}