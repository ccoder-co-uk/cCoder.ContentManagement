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

public partial class ResourceProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteAllMatchingVersionsOnceWhenDeleteAllAsync()
    {
        // Given
        Resource rootResource = CreateRandomResource(id: 42, culture: string.Empty);
        Resource secondVersion = CreateRandomResource(id: 43, culture: string.Empty);
        secondVersion.AppId = rootResource.AppId;
        secondVersion.Key = rootResource.Key;
        secondVersion.Name = rootResource.Name;

        resourceServiceMock.Setup(x => x.Get(rootResource.Id)).Returns(rootResource);

        resourceServiceMock
            .Setup(x => x.GetAll())
            .Returns(new[] { rootResource, secondVersion }.AsQueryable());

        resourceServiceMock
            .Setup(x => x.DeleteAsync(rootResource.Id))
            .Returns(ValueTask.CompletedTask);

        resourceServiceMock
            .Setup(x => x.DeleteAsync(secondVersion.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await resourceProcessingService.DeleteAllAsync(new[] { rootResource, secondVersion });

        // Then
        resourceServiceMock.Verify(x => x.Get(rootResource.Id), Times.Once);
        resourceServiceMock.Verify(x => x.GetAll(), Times.Exactly(2));
        resourceServiceMock.Verify(x => x.DeleteAsync(rootResource.Id), Times.Once);
        resourceServiceMock.Verify(x => x.DeleteAsync(secondVersion.Id), Times.Once);
        resourceServiceMock.VerifyNoOtherCalls();
    }

}















