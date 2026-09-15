// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldResetIdentityAndAddWhenImportItemIsNew()
    {
        // Given
        CommonObject incoming = CreateRandomCommonObject(type: "Core/Other");
        incoming.Id = 99;
        incoming.Version = 9;

        commonObjectServiceMock
            .Setup(expression: service => service.AddCommonObjectAsync(
                newCommonObject: incoming,
                userId: CurrentUserId))
            .ReturnsAsync(value: incoming);

        // When
        OperationResult<CommonObject>[] results = (await commonObjectProcessingService
            .AddAllCommonObjectsAsync(
                newCommonObjects: [incoming],
                latestCommonObjects: [],
                userId: CurrentUserId))
            .ToArray();

        // Then
        results.Should()
            .ContainSingle(predicate: result => result.Success && result.Item == incoming);

        incoming.Id.Should()
            .Be(expected: 0);

        incoming.Version.Should()
            .Be(expected: 1);

        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }

    [Fact]
    public async Task ShouldPromoteVersionAndAddWhenImportItemIsNewer()
    {
        // Given
        CommonObject existing = CreateRandomCommonObject(type: "Core/Other");
        existing.Version = 4;
        existing.CreatedOn = DateTimeOffset.UtcNow.AddHours(hours: -2);
        existing.LastUpdated = existing.CreatedOn;
        CommonObject incoming = CreateRandomCommonObject(type: existing.Type);
        incoming.Name = existing.Name;
        incoming.Key = existing.Key;
        incoming.Culture = existing.Culture;
        incoming.CreatedOn = DateTimeOffset.UtcNow;
        incoming.LastUpdated = incoming.CreatedOn;

        commonObjectServiceMock
            .Setup(expression: service => service.GetAllCommonObject(ignoreFilters: false))
            .Returns(value: new[] { existing }.AsQueryable());

        commonObjectServiceMock
            .Setup(expression: service => service.AddCommonObjectAsync(
                newCommonObject: incoming,
                userId: CurrentUserId))
            .ReturnsAsync(value: incoming);

        // When
        OperationResult<CommonObject>[] results = (await commonObjectProcessingService
            .AddAllCommonObjectsAsync(
                newCommonObjects: [incoming],
                latestCommonObjects: [existing],
                userId: CurrentUserId))
            .ToArray();

        // Then
        results.Should()
            .ContainSingle(predicate: result => result.Success);

        incoming.Id.Should()
            .Be(expected: 0);

        incoming.Version.Should()
            .Be(expected: 5);

        commonObjectServiceMock.Verify(
            expression: service => service.GetAllCommonObject(ignoreFilters: false),
            times: Times.Exactly(callCount: 2));

        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }

    [Fact]
    public async Task ShouldReturnNoResultsWhenImportItemIsNotNewer()
    {
        // Given
        CommonObject existing = CreateRandomCommonObject(type: "Core/Other");
        CommonObject incoming = CreateRandomCommonObject(type: existing.Type);
        incoming.Name = existing.Name;
        incoming.Key = existing.Key;
        incoming.Culture = existing.Culture;
        incoming.CreatedOn = existing.CreatedOn;
        incoming.LastUpdated = existing.LastUpdated;

        // When
        OperationResult<CommonObject>[] results = (await commonObjectProcessingService
            .AddAllCommonObjectsAsync(
                newCommonObjects: [incoming],
                latestCommonObjects: [existing],
                userId: CurrentUserId))
            .ToArray();

        // Then
        results.Should()
            .BeEmpty();

        VerifyNoOtherCommonObjectServiceCalls();
    }
}