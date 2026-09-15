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
    public async Task ShouldStampUpdateAuditFieldsAndDelegateToBrokerWhenUpdateAsync()
    {
        // Given
        const string userId = "test-user";
        CommonObject commonObject = CreateRandomCommonObject(id: 7);
        DateTimeOffset originalCreatedOn = commonObject.CreatedOn;
        string originalCreatedBy = commonObject.CreatedBy;
        CommonObject submitted = null;

        commonObjectBrokerMock
            .Setup(expression: broker => broker.UpdateCommonObjectAsync(
                updatedCommonObject: It.IsAny<CommonObject>()))
            .Callback<CommonObject>(action: item => submitted = item)
            .ReturnsAsync(valueFunction: (CommonObject item) => item);

        // When
        CommonObject result = await commonObjectService.UpdateCommonObjectAsync(
            updatedCommonObject: commonObject,
            userId: userId);

        // Then
        result.Should()
            .BeSameAs(expected: commonObject);

        submitted.Should()
            .NotBeSameAs(unexpected: commonObject);

        submitted.CreatedOn.Should()
            .Be(expected: originalCreatedOn);

        submitted.CreatedBy.Should()
            .Be(expected: originalCreatedBy);

        submitted.LastUpdatedBy.Should()
            .Be(expected: userId);

        submitted.LastUpdated.Should()
            .BeCloseTo(
                nearbyTime: DateTimeOffset.UtcNow,
                precision: TimeSpan.FromSeconds(value: 5));

        commonObject.LastUpdatedBy.Should()
            .Be(expected: userId);

        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}