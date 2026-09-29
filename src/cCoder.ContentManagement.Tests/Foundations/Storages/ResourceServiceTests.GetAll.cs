// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    [Fact]
    public void ShouldReturnQueryWhenGetAll()
    {
        // Given

        // When
        Resource[] expectedItems =
        {
            CreateRandomResource(id: 1, appId: 7),
        };

        IQueryable<CmsDataModels.Resource> resources = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        resourceBrokerMock.Setup(expression: x => x.GetAllResources())
            .Returns(value: resources);

        IQueryable<Resource> result = resourceService.GetAllResource();

        // Then
        Assert.Single(collection: result);
    }

}