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
        authorizationBrokerMock
            .Setup(x => x.Authorize(id, "app_delete"));
        appProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { app }.AsQueryable());
        appEventProcessingServiceMock
            .Setup(x => x.RaiseAppDeleteEventAsync(app))
            .Returns(ValueTask.CompletedTask);
        appProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        authorizationBrokerMock.Verify(x => x.Authorize(id, "app_delete"), Times.Once);
        appProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        appEventProcessingServiceMock.Verify(x => x.RaiseAppDeleteEventAsync(app), Times.Once);
        appProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }

}
















