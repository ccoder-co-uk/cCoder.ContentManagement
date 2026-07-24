// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;
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

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        App app = CreateRandomApp();
        app.Id = id;
        app.Roles = [new Role { Id = Guid.NewGuid(), AppId = id, Users = [] }];

        authorizationProcessingServiceMock
            .Setup(expression: x => x.Authorize(appId: id, privilege: "app_delete"));

        appProcessingServiceMock.Setup(expression: x => x.GetAllApp(ignoreFilters: true))
            .Returns(value: new[] { app }.AsQueryable());

        appEventProcessingServiceMock
            .Setup(expression: x => x.RaiseAppDeleteEventAsync(app: app))
            .Returns(value: ValueTask.CompletedTask);

        appProcessingServiceMock.Setup(expression: x => x.DeleteAsync(appId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(appId: id);

        // Then
        authorizationProcessingServiceMock.Verify(expression: x => x.Authorize(appId: id, privilege: "app_delete"), times: Times.Once);
        appProcessingServiceMock.Verify(expression: x => x.GetAllApp(ignoreFilters: true), times: Times.Once);
        appEventProcessingServiceMock.Verify(expression: x => x.RaiseAppDeleteEventAsync(app: app), times: Times.Once);
        appProcessingServiceMock.Verify(expression: x => x.DeleteAsync(appId: id), times: Times.Once);
    }

}