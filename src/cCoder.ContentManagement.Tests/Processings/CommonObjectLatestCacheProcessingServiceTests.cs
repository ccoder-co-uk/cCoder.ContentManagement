// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public sealed partial class CommonObjectLatestCacheProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateWhenGetLatestCommonObjects()
    {
        // Given
        CommonObject[] items = [new() { Id = 42 }];
        Mock<ICommonObjectLatestCacheService> foundationService = new(behavior: MockBehavior.Strict);

        foundationService
            .Setup(expression: service => service.GetLatestCommonObjects())
            .Returns(value: items);

        CommonObjectLatestCacheProcessingService service = new(
            commonObjectLatestCacheService: foundationService.Object);

        // When
        IEnumerable<CommonObject> actual = service.GetLatestCommonObjects();

        // Then
        actual.Should()
            .BeSameAs(expected: items);

        foundationService.VerifyAll();
        foundationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRefreshFoundationWhenChangedCountIsPositive()
    {
        // Given
        Mock<ICommonObjectLatestCacheService> foundationService = new(behavior: MockBehavior.Strict);

        foundationService.Setup(
            expression: service => service.RefreshCommonObjects());

        CommonObjectLatestCacheProcessingService service = new(
            commonObjectLatestCacheService: foundationService.Object);

        // When
        service.RefreshCommonObjects(changedCommonObjectCount: 2);

        // Then
        foundationService.VerifyAll();
        foundationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldNotRefreshFoundationWhenChangedCountIsZero()
    {
        // Given
        Mock<ICommonObjectLatestCacheService> foundationService = new(behavior: MockBehavior.Strict);

        CommonObjectLatestCacheProcessingService service = new(
            commonObjectLatestCacheService: foundationService.Object);

        // When
        service.RefreshCommonObjects(changedCommonObjectCount: 0);

        // Then
        foundationService.VerifyNoOtherCalls();
    }
}