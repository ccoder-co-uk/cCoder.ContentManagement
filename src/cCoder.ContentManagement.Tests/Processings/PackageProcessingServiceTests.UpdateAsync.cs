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
        packageServiceMock.Setup(service => service.UpdatePackageAsync(package)).ReturnsAsync(package);

        // When
        Package result = await packageProcessingService.UpdatePackageAsync(package);

        // Then
        Assert.Same(package, result);
        packageServiceMock.Verify(service => service.UpdatePackageAsync(package), Times.Once);
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

        packageServiceMock.Setup(service => service.UpdatePackageAsync(package)).ReturnsAsync(package);
        packageItemServiceMock
            .Setup(service => service.GetAllPackageItem(false))
            .Returns(new[] { existingItem }.AsQueryable());
        packageItemServiceMock
            .Setup(service => service.DeleteAllPackageItemAsync(
                It.Is<IEnumerable<PackageItem>>(items => items.Single() == existingItem)))
            .Returns(ValueTask.CompletedTask);

        // When
        Package result = await packageProcessingService.UpdatePackageAsync(package);

        // Then
        Assert.Same(package, result);
        packageServiceMock.Verify(service => service.UpdatePackageAsync(package), Times.Once);
        packageItemServiceMock.Verify(service => service.GetAllPackageItem(false), Times.Once);
        packageItemServiceMock.Verify(
            service => service.DeleteAllPackageItemAsync(
                It.Is<IEnumerable<PackageItem>>(items => items.Single() == existingItem)),
            Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }
}
