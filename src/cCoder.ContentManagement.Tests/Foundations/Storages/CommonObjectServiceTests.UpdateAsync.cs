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
        const string userId = "test-user";
        CommonObject commonObject = CreateRandomCommonObject(id: 7);
        DateTimeOffset originalCreatedOn = commonObject.CreatedOn;
        string originalCreatedBy = commonObject.CreatedBy;
        CommonObject submitted = null;

        commonObjectBrokerMock
            .Setup(broker => broker.UpdateCommonObjectAsync(It.IsAny<CommonObject>()))
            .Callback<CommonObject>(item => submitted = item)
            .ReturnsAsync((CommonObject item) => item);

        CommonObject result = await commonObjectService.UpdateCommonObjectAsync(
            updatedCommonObject: commonObject,
            userId: userId);

        result.Should().BeSameAs(commonObject);
        submitted.Should().NotBeSameAs(commonObject);
        submitted.CreatedOn.Should().Be(originalCreatedOn);
        submitted.CreatedBy.Should().Be(originalCreatedBy);
        submitted.LastUpdatedBy.Should().Be(userId);
        submitted.LastUpdated.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        commonObject.LastUpdatedBy.Should().Be(userId);
        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}