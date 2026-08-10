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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        App entity = CreateRandomApp();

        appProcessingServiceMock.Setup(expression: x => x.UpdateAppAsync(updatedApp: entity))
            .ReturnsAsync(value: entity);

        appEventProcessingServiceMock
            .Setup(expression: x => x.RaiseAppUpdateEventAsync(app: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        App result = await orchestrationService.UpdateAppAsync(updatedApp: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        appProcessingServiceMock.Verify(expression: x => x.UpdateAppAsync(updatedApp: entity), times: Times.Once);
        appEventProcessingServiceMock.Verify(expression: x => x.RaiseAppUpdateEventAsync(app: entity), times: Times.Once);
        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRaiseUpdateEventWithPostedGraphWhenStoredAppIsFlatAsync()
    {
        // Given
        App postedApp = CreateRandomApp();
        App storedApp = new() { Id = postedApp.Id, Name = postedApp.Name, Domain = postedApp.Domain };

        appProcessingServiceMock
            .Setup(expression: service => service.UpdateAppAsync(updatedApp: postedApp))
            .ReturnsAsync(value: storedApp);

        appEventProcessingServiceMock
            .Setup(expression: service => service.RaiseAppUpdateEventAsync(app: postedApp))
            .Returns(value: ValueTask.CompletedTask);

        // When
        App result = await orchestrationService
            .UpdateAppAsync(updatedApp: postedApp);

        // Then
        result.Should()
            .BeSameAs(expected: storedApp);

        appEventProcessingServiceMock.Verify(
            expression: service => service.RaiseAppUpdateEventAsync(app: postedApp),
            times: Times.Once);

        appProcessingServiceMock.Verify(
            expression: service => service.UpdateAppAsync(updatedApp: postedApp),
            times: Times.Once);

        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}