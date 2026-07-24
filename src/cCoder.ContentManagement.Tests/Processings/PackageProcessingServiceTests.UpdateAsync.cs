// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageProcessingServiceTests
{
    [Fact]
    public async Task ShouldNotTouchPackageItemsWhenUpdateAsyncGivenNullItems()
    {
        // Given
        Package package = CreateRandomPackage();
        package.Items = null;

        packageServiceMock.Setup(expression: service => service.UpdatePackageAsync(updatedPackage: package))
            .ReturnsAsync(value: package);

        // When
        Package result = await packageProcessingService.UpdatePackageAsync(updatedPackage: package);

        // Then
        Assert.Same(expected: package, actual: result);
        packageServiceMock.Verify(expression: service => service.UpdatePackageAsync(updatedPackage: package), times: Times.Once);
    }
}