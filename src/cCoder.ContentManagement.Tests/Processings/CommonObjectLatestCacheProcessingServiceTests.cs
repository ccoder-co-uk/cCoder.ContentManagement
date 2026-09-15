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

public sealed class CommonObjectLatestCacheProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateWhenGetLatestCommonObjects()
    {
        CommonObject[] items = [new() { Id = 42 }];
        Mock<ICommonObjectLatestCacheService> foundationService = new(MockBehavior.Strict);
        foundationService.Setup(service => service.GetLatestCommonObjects()).Returns(items);
        CommonObjectLatestCacheProcessingService service = new(foundationService.Object);

        IEnumerable<CommonObject> actual = service.GetLatestCommonObjects();

        actual.Should().BeSameAs(items);
        foundationService.VerifyAll();
        foundationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRefreshFoundationWhenChangedCountIsPositive()
    {
        Mock<ICommonObjectLatestCacheService> foundationService = new(MockBehavior.Strict);
        foundationService.Setup(service => service.RefreshCommonObjects());
        CommonObjectLatestCacheProcessingService service = new(foundationService.Object);

        service.RefreshCommonObjects(changedCommonObjectCount: 2);

        foundationService.VerifyAll();
        foundationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldNotRefreshFoundationWhenChangedCountIsZero()
    {
        Mock<ICommonObjectLatestCacheService> foundationService = new(MockBehavior.Strict);
        CommonObjectLatestCacheProcessingService service = new(foundationService.Object);

        service.RefreshCommonObjects(changedCommonObjectCount: 0);

        foundationService.VerifyNoOtherCalls();
    }
}