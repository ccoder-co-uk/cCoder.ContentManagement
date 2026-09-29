// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo(id: 7);

        pageInfoBrokerMock.Setup(expression: x => x.GetAllPageInfo())
            .Returns(value: new[] { ToDataPageInfo(pageInfo: pageInfo) }.AsQueryable());

        // When
        PageInfo result = pageInfoService.GetPageInfo(pageInfoId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: pageInfo);

        pageInfoBrokerMock.Verify(expression: x => x.GetAllPageInfo(), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
    }

}