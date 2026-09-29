// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    [Fact]
    public void ShouldReturnEntityWhenGet()
    {
        // Given

        // When
        Resource resource = CreateRandomResource(id: 5, appId: 7);

        resourceBrokerMock.Setup(expression: x => x.GetAllResources())
            .Returns(value: new[] { resource }.AsQueryable());

        Resource result = resourceService.GetResource(resourceId: 5);

        // Then
        Assert.Equivalent(expected: resource, actual: result);
    }

}