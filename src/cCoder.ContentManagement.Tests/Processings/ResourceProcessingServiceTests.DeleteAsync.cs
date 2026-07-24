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
        User currentUser = TestUsers.WithPrivilege(privilege: "resource_delete", appId: rootResource.AppId);

        resourceServiceMock.Setup(expression: x => x.GetResource(resourceId: rootResource.Id))
            .Returns(value: rootResource);

        resourceServiceMock
            .Setup(expression: x => x.GetAllResource())
            .Returns(value: new[] { rootResource, secondVersion }.AsQueryable());

        resourceServiceMock
            .Setup(expression: x => x.DeleteAsync(resourceId: rootResource.Id))
            .Returns(value: ValueTask.CompletedTask);

        resourceServiceMock
            .Setup(expression: x => x.DeleteAsync(resourceId: secondVersion.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await resourceProcessingService.DeleteAsync(resourceId: rootResource.Id);

        // Then
        resourceServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: rootResource.Id), times: Times.Once);
        resourceServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: secondVersion.Id), times: Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteSingleVersionWhenDeleteAsync()
    {
        // Given
        Resource resource = CreateRandomResource(id: 42, culture: "en-GB");

        resourceServiceMock.Setup(expression: x => x.GetResource(resourceId: resource.Id))
            .Returns(value: resource);

        resourceServiceMock.Setup(expression: x => x.DeleteAsync(resourceId: resource.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await resourceProcessingService.DeleteAsync(resourceId: resource.Id);

        // Then
        resourceServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: resource.Id), times: Times.Once);
    }

    [Fact]
    public async Task ShouldReturnWithoutDeletingWhenResourceDoesNotExistForDeleteAsync()
    {
        // Given
        int resourceId = 42;

        resourceServiceMock.Setup(expression: x => x.GetResource(resourceId: resourceId))
            .Returns(value: (Resource)null!);

        // When
        await resourceProcessingService.DeleteAsync(resourceId: resourceId);

        // Then
        resourceServiceMock.Verify(expression: x => x.GetResource(resourceId: resourceId), times: Times.Once);
        resourceServiceMock.VerifyNoOtherCalls();
    }

}
