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

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingWhenAddAsync()
    {
        // Given
        App entity = CreateRandomApp();
        appProcessingServiceMock
            .Setup(x => x.AddAsync(entity))
            .ReturnsAsync((App app) => app);

        // When
        App result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        appProcessingServiceMock.Verify(x => x.AddAsync(It.IsAny<App>()), Times.Once);
        appProcessingServiceMock.VerifyNoOtherCalls();
        appEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}





















