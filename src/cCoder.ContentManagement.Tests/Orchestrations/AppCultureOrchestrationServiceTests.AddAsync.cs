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

public partial class AppCultureOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        AppCulture entity = CreateRandomAppCulture();
        appCultureProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        appCultureEventProcessingServiceMock
            .Setup(x => x.RaiseAppCultureAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        AppCulture result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        appCultureProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        appCultureEventProcessingServiceMock.Verify(x => x.RaiseAppCultureAddEventAsync(entity), Times.Once);
    }

}




















