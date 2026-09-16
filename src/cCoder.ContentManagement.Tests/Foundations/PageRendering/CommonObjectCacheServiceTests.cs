// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.PageRendering;

public sealed partial class CommonObjectCacheServiceTests
{
    [Fact]
    public void GetCommonObjectCacheSnapshot_WhenCalled_ReadsUtilityCache()
    {
        // Given
        CommonObjectCacheSnapshot snapshot = new();
        Mock<ICacheBroker> cacheBroker = new();

        cacheBroker
            .Setup(expression: broker =>
                broker.Get<CommonObjectCacheSnapshot>(
                    key: "ContentManagement.CommonObjects"))
            .Returns(value: snapshot);

        CommonObjectCacheService service = new(
            cacheBroker: cacheBroker.Object);

        // When
        CommonObjectCacheSnapshot actual = service
            .GetCommonObjectCacheSnapshot();

        // Then
        actual.Should()
            .BeSameAs(expected: snapshot);

        cacheBroker.VerifyAll();
    }
}