// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.Data.Models;
using cCoder.ContentManagement.Models.Caching;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public sealed partial class CommonObjectLatestCacheProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateWhenGetCommonObjectCacheSnapshot()
    {
        // Given
        CommonObjectCacheSnapshot snapshot = new()
        {
            LatestSet = [new CommonObject { Id = 42 }]
        };

        Mock<ICommonObjectLatestCacheService> foundationService = new(behavior: MockBehavior.Strict);

        foundationService
            .Setup(expression: service => service.GetCommonObjectCacheSnapshot())
            .Returns(value: snapshot);

        CommonObjectLatestCacheProcessingService service = new(
            commonObjectLatestCacheService: foundationService.Object);

        // When
        CommonObjectCacheSnapshot actual = service
            .GetCommonObjectCacheSnapshot();

        // Then
        actual.Should()
            .BeSameAs(expected: snapshot);

        foundationService.VerifyAll();
        foundationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRefreshFoundationWhenChangedCountIsPositive()
    {
        // Given
        Mock<ICommonObjectLatestCacheService> foundationService = new(behavior: MockBehavior.Strict);

        foundationService.Setup(
            expression: service => service.LoadCommonObjectCacheSnapshot())
            .Returns(value: new CommonObjectCacheSnapshot());

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