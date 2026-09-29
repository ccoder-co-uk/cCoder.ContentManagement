// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
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
        await resourceProcessingService.DeleteAllResourceAsync(deletedResource: new[] { rootResource, secondVersion });

        // Then
        resourceServiceMock.Verify(expression: x => x.GetResource(resourceId: rootResource.Id), times: Times.Once);
        resourceServiceMock.Verify(expression: x => x.GetAllResource(), times: Times.Exactly(callCount: 2));
        resourceServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: rootResource.Id), times: Times.Once);
        resourceServiceMock.Verify(expression: x => x.DeleteAsync(resourceId: secondVersion.Id), times: Times.Once);
        resourceServiceMock.VerifyNoOtherCalls();
    }

}