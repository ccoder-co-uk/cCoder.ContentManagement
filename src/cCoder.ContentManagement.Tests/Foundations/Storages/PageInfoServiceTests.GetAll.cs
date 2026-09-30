// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;
using DataPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();
        IQueryable<DataPageInfo> pageInfos = new[] { ToDataPageInfo(pageInfo: pageInfo) }.AsQueryable();

        pageInfoBrokerMock.Setup(expression: x => x.GetAllPageInfos())
            .Returns(value: pageInfos);

        // When
        IQueryable<PageInfo> result = pageInfoService.GetAllPageInfos();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: [pageInfo]);

        pageInfoBrokerMock.Verify(expression: x => x.GetAllPageInfos(), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
    }

}