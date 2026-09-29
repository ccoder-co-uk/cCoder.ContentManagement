// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Resource resource = CreateRandomResource(id: 7);

        resourceServiceMock.Setup(expression: x => x.AddResourceAsync(newResource: resource))
            .ReturnsAsync(value: resource);

        // When
        Resource result = await resourceProcessingService.AddResourceAsync(newResource: resource);

        // Then
        Assert.Same(expected: resource, actual: result);
        resourceServiceMock.Verify(expression: x => x.AddResourceAsync(newResource: resource), times: Times.Once);
    }

}