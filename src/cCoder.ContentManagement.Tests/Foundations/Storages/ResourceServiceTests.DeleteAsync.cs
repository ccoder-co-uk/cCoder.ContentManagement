// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given

        Resource resource = CreateRandomResource(id: 5, appId: 7);

        resourceBrokerMock.Setup(expression: x => x.GetAllResources())
            .Returns(value: new[] { resource }.AsQueryable());

        resourceBrokerMock.Setup(expression: x => x.DeleteResourceAsync(deletedResource: It.IsAny<CmsDataModels.Resource>()))
            .ReturnsAsync(value: 1);

        // When
        await resourceService.DeleteAsync(resourceId: 5);

        // Then
        resourceBrokerMock.Verify(expression: x => x.DeleteResourceAsync(deletedResource: It.Is<CmsDataModels.Resource>(match: actual => actual.Id == resource.Id)), times: Times.Once);
    }

}