// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToLatestCacheWhenGetLatestCommonObjects()
    {
        CommonObject[] items = [CreateRandomCommonObject()];
        commonObjectServiceMock.Setup(service => service.GetLatestCommonObjects())
            .Returns(items);

        IEnumerable<CommonObject> actual =
            commonObjectProcessingService.GetLatestCommonObjects();

        actual.Should().BeSameAs(items);
        commonObjectServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldDelegateToLatestCacheWhenRefreshCommonObjects()
    {
        commonObjectServiceMock.Setup(service => service.RefreshCommonObjects());

        commonObjectProcessingService.RefreshCommonObjects();

        commonObjectServiceMock.VerifyAll();
    }
}