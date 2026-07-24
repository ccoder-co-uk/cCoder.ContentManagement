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
        packageItemServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteExistingPackageItemsWhenUpdateAsyncGivenEmptyItems()
    {
        // Given
        Package package = CreateRandomPackage();
        package.Items = [];
        PackageItem existingItem = CreateRandomPackageItem();
        existingItem.PackageId = package.Id;

        packageServiceMock.Setup(expression: service => service.UpdatePackageAsync(updatedPackage: package))
            .ReturnsAsync(value: package);

        packageItemServiceMock
            .Setup(expression: service => service.GetAllPackageItem(ignoreFilters: false))
            .Returns(value: new[] { existingItem }.AsQueryable());

        packageItemServiceMock
            .Setup(expression: service => service.DeleteAllPackageItemAsync(
deletedPackageItem: It.Is<IEnumerable<PackageItem>>(match: items => items.Single() == existingItem)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Package result = await packageProcessingService.UpdatePackageAsync(updatedPackage: package);

        // Then
        Assert.Same(expected: package, actual: result);
        packageServiceMock.Verify(expression: service => service.UpdatePackageAsync(updatedPackage: package), times: Times.Once);
        packageItemServiceMock.Verify(expression: service => service.GetAllPackageItem(ignoreFilters: false), times: Times.Once);

        packageItemServiceMock.Verify(
expression: service => service.DeleteAllPackageItemAsync(
deletedPackageItem: It.Is<IEnumerable<PackageItem>>(match: items => items.Single() == existingItem)),
times: Times.Once);

        packageItemServiceMock.VerifyNoOtherCalls();
    }
}