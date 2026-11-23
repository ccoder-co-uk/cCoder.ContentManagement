// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    [Fact]
    public async Task ShouldStampAndPersistAllCommonObjectsInOneBatchAsync()
    {
        // Given
        const string userId = "test-user";

        CommonObject[] commonObjects =
        [
            CreateRandomCommonObject(id: 0),
            CreateRandomCommonObject(id: 0)
        ];

        CommonObject[] submitted = null;

        commonObjectBrokerMock
            .Setup(expression: broker => broker.AddAllCommonObjectsAsync(
                newCommonObjects: It.IsAny<CommonObject[]>()))
            .Callback<CommonObject[]>(action: items => submitted = items)
            .ReturnsAsync(valueFunction: (CommonObject[] items) => items);

        // When
        CommonObject[] results = await commonObjectService.AddAllCommonObjectsAsync(
            newCommonObjects: commonObjects,
            userId: userId);

        // Then
        results
            .Should()
            .BeSameAs(expected: commonObjects);

        submitted
            .Should()
            .HaveCount(expected: 2)
            .And.OnlyContain(predicate: item =>
                item.CreatedBy == userId
                && item.LastUpdatedBy == userId);

        submitted
            .Should()
            .NotContain(unexpected: commonObjects);

        commonObjectBrokerMock.Verify(
            expression: broker => broker.AddAllCommonObjectsAsync(
                newCommonObjects: It.IsAny<CommonObject[]>()),
            times: Times.Once);

        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}