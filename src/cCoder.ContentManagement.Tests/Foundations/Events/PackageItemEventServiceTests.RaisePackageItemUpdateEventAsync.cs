// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Packaging;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PackageItemEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePackageItemUpdateEventAsync()
    {
        // Given
        PackageItem entity = new();
        EventMessage<cCoder.Data.Models.Packaging.PackageItem> actualMessage = null;

        packageItemEventBrokerMock
            .Setup(expression: x => x.RaisePackageItemUpdateEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Packaging.PackageItem>>()))
            .Callback<EventMessage<cCoder.Data.Models.Packaging.PackageItem>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePackageItemUpdateEventAsync(
            packageItem: entity,
            userId: CurrentUserId);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        packageItemEventBrokerMock.Verify(
expression: x => x.RaisePackageItemUpdateEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Packaging.PackageItem>>()),
times: Times.Once
        );

        packageItemEventBrokerMock.VerifyNoOtherCalls();
    }

}