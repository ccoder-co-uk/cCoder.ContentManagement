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
    public async Task ShouldDeleteAllMatchingVersionsWhenDeleteAsync()
    {
        // Given
        Resource rootResource = CreateRandomResource(id: 42, culture: string.Empty);
        Resource secondVersion = CreateRandomResource(id: 43, culture: string.Empty);
        secondVersion.AppId = rootResource.AppId;
        secondVersion.Key = rootResource.Key;
        secondVersion.Name = rootResource.Name;
        User currentUser = TestUsers.WithPrivilege("resource_delete", rootResource.AppId);
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
        await resourceProcessingService.DeleteAsync(rootResource.Id);

        // Then
        resourceServiceMock.Verify(x => x.DeleteAsync(rootResource.Id), Times.Once);
        resourceServiceMock.Verify(x => x.DeleteAsync(secondVersion.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteSingleVersionWhenDeleteAsync()
    {
        // Given
        Resource resource = CreateRandomResource(id: 42, culture: "en-GB");
        resourceServiceMock.Setup(x => x.Get(resource.Id)).Returns(resource);
        resourceServiceMock.Setup(x => x.DeleteAsync(resource.Id)).Returns(ValueTask.CompletedTask);

        // When
        await resourceProcessingService.DeleteAsync(resource.Id);

        // Then
        resourceServiceMock.Verify(x => x.DeleteAsync(resource.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnWithoutDeletingWhenResourceDoesNotExistForDeleteAsync()
    {
        // Given
        int resourceId = 42;
        resourceServiceMock.Setup(x => x.Get(resourceId)).Returns((Resource)null!);

        // When
        await resourceProcessingService.DeleteAsync(resourceId);

        // Then
        resourceServiceMock.Verify(x => x.Get(resourceId), Times.Once);
        resourceServiceMock.VerifyNoOtherCalls();
    }

}















