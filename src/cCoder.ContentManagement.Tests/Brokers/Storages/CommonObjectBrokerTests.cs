// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Exposures;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers.Storages;

public sealed partial class CommonObjectBrokerTests
{
    [Fact]
    public void GetLatestCommonObjectsPaged_WhenCalled_ReadsLatestObjectsFromDatabase()
    {
        // Given

        CommonObject olderCommonObject = CreateCommonObject(
            id: 1,
            version: 1);

        CommonObject latestCommonObject = CreateCommonObject(
            id: 2,
            version: 2);

        Mock<ICommonObjectCacheManager> managerMock = new(
            behavior: MockBehavior.Strict);

        managerMock.Setup(expression: manager => manager.Get(
                fromCache: false,
                ignoreFilters: true))
            .Returns(value: new[]
            {
                olderCommonObject,
                latestCommonObject
            }.AsQueryable());

        CommonObjectBroker broker = new(
            commonObjectCacheManager: managerMock.Object);

        // When

        CommonObject[] result = broker.GetLatestCommonObjectsPaged();

        // Then

        result
            .Should()
            .Equal(expected: [latestCommonObject]);

        managerMock.Verify(expression: manager => manager.Get(
            fromCache: false,
            ignoreFilters: true), times: Times.Once);
    }

    private static CommonObject CreateCommonObject(
        int id,
        int version) =>
        new CommonObject
        {
            Id = id,
            Name = "Shared object",
            Type = "ContentManagement/Component",
            Version = version,
            Culture = string.Empty,
            Key = string.Empty,
            Json = "{}"
        };
}