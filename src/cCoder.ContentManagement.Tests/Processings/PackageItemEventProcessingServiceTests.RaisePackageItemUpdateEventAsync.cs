// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageItemEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePackageItemUpdateEventAsync()
    {
        // Given
        PackageItem entity = CreateRandomPackageItem();

        packageItemEventServiceMock
            .Setup(expression: x => x.RaisePackageItemUpdateEventAsync(
                entity: entity,
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePackageItemUpdateEventAsync(
            packageItem: entity,
            userId: CurrentUserId);

        // Then
        packageItemEventServiceMock.Verify(expression: x => x.RaisePackageItemUpdateEventAsync(
            entity: entity,
            userId: CurrentUserId), times: Times.Once);

        packageItemEventServiceMock.VerifyNoOtherCalls();
    }

}