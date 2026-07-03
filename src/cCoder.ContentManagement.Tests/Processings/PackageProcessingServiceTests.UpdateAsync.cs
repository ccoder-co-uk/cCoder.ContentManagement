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
        packageServiceMock.Setup(service => service.UpdateAsync(package)).ReturnsAsync(package);

        // When
        Package result = await packageProcessingService.UpdateAsync(package);

        // Then
        Assert.Same(package, result);
        packageServiceMock.Verify(service => service.UpdateAsync(package), Times.Once);
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

        packageServiceMock.Setup(service => service.UpdateAsync(package)).ReturnsAsync(package);
        packageItemServiceMock
            .Setup(service => service.GetAll(false))
            .Returns(new[] { existingItem }.AsQueryable());
        packageItemServiceMock
            .Setup(service => service.DeleteAllAsync(
                It.Is<IEnumerable<PackageItem>>(items => items.Single() == existingItem)))
            .Returns(ValueTask.CompletedTask);

        // When
        Package result = await packageProcessingService.UpdateAsync(package);

        // Then
        Assert.Same(package, result);
        packageServiceMock.Verify(service => service.UpdateAsync(package), Times.Once);
        packageItemServiceMock.Verify(service => service.GetAll(false), Times.Once);
        packageItemServiceMock.Verify(
            service => service.DeleteAllAsync(
                It.Is<IEnumerable<PackageItem>>(items => items.Single() == existingItem)),
            Times.Once);
        packageItemServiceMock.VerifyNoOtherCalls();
    }
}
