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
    public void ShouldDelegateToBrokerWhenGetLatestCommonObjects()
    {
        CommonObject[] items = [CreateRandomCommonObject()];
        commonObjectBrokerMock.Setup(broker => broker.GetLatestCachedCommonObjects())
            .Returns(items);

        IEnumerable<CommonObject> actual = commonObjectService.GetLatestCommonObjects();

        actual.Should().BeSameAs(items);
        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldDelegateToBrokerWhenRefreshCommonObjects()
    {
        commonObjectBrokerMock.Setup(broker => broker.RefreshCommonObjectCache());

        commonObjectService.RefreshCommonObjects();

        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}