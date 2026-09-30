// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageServiceTests
{
    [Fact]
    public void ShouldReturnPagesWhenGetAll()
    {
        // Given
        Page[] expectedItems = [CreateRandomPage(id: 1)];

        IQueryable<CmsDataModels.Page> pages = expectedItems.Select(selector: item => item)
            .AsQueryable();

        pageBrokerMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: pages);

        // When
        IQueryable<Page> result = pageService.GetAllPages();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        pageBrokerMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
    }

}