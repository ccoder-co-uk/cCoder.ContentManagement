// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageServiceTests
{
    [Fact]
    public void ShouldReturnPageWhenGet()
    {
        // Given
        Page page = CreateRandomPage(id: 5);

        pageBrokerMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { page }.AsQueryable());

        // When
        Page result = pageService.GetPage(pageId: 5);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: page);

        pageBrokerMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
    }

}