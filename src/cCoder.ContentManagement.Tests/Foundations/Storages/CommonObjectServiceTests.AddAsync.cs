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
        const string userId = "test-user";
        CommonObject commonObject = CreateRandomCommonObject(id: 0);
        CommonObject submitted = null;

        commonObjectBrokerMock
            .Setup(broker => broker.AddCommonObjectAsync(It.IsAny<CommonObject>()))
            .Callback<CommonObject>(item => submitted = item)
            .ReturnsAsync((CommonObject item) => item);

        CommonObject result = await commonObjectService.AddCommonObjectAsync(
            newCommonObject: commonObject,
            userId: userId);

        result.Should().BeSameAs(commonObject);
        submitted.Should().NotBeSameAs(commonObject);
        submitted.CreatedBy.Should().Be(userId);
        submitted.LastUpdatedBy.Should().Be(userId);
        submitted.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        submitted.LastUpdated.Should().Be(submitted.CreatedOn);
        commonObject.CreatedBy.Should().Be(userId);
        commonObject.LastUpdatedBy.Should().Be(userId);
        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}