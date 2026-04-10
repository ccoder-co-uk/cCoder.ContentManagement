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

public partial class ComponentEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseComponentAddEventAsync()
    {
        // Given
        Component entity = CreateRandomComponent();
        componentEventServiceMock
            .Setup(x => x.RaiseComponentAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseComponentAddEventAsync(entity);

        // Then
        componentEventServiceMock.Verify(x => x.RaiseComponentAddEventAsync(entity), Times.Once);
        componentEventServiceMock.VerifyNoOtherCalls();
    }

}
















