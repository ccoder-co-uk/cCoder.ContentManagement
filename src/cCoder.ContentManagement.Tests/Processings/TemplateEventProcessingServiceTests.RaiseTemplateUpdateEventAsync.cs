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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseTemplateUpdateEventAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();
        templateEventServiceMock
            .Setup(x => x.RaiseTemplateUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseTemplateUpdateEventAsync(entity);

        // Then
        templateEventServiceMock.Verify(x => x.RaiseTemplateUpdateEventAsync(entity), Times.Once);
        templateEventServiceMock.VerifyNoOtherCalls();
    }

}
















