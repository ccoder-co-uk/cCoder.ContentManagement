// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    [Fact]
    public async Task ShouldStampAuditFieldsAndDelegateToBrokerWhenAddAsync()
    {
        // Given
        const string userId = "test-user";
        CommonObject commonObject = CreateRandomCommonObject(id: 0);
        CommonObject submitted = null;

        commonObjectBrokerMock
            .Setup(expression: broker => broker.AddCommonObjectAsync(
                newCommonObject: It.IsAny<CommonObject>()))
            .Callback<CommonObject>(action: item => submitted = item)
            .ReturnsAsync(valueFunction: (CommonObject item) => item);

        // When
        CommonObject result = await commonObjectService.AddCommonObjectAsync(
            newCommonObject: commonObject,
            userId: userId);

        // Then
        result.Should()
            .BeSameAs(expected: commonObject);

        submitted.Should()
            .NotBeSameAs(unexpected: commonObject);

        submitted.CreatedBy.Should()
            .Be(expected: userId);

        submitted.LastUpdatedBy.Should()
            .Be(expected: userId);

        submitted.CreatedOn.Should()
            .BeCloseTo(
                nearbyTime: DateTimeOffset.UtcNow,
                precision: TimeSpan.FromSeconds(value: 5));

        submitted.LastUpdated.Should()
            .Be(expected: submitted.CreatedOn);

        commonObject.CreatedBy.Should()
            .Be(expected: userId);

        commonObject.LastUpdatedBy.Should()
            .Be(expected: userId);

        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}