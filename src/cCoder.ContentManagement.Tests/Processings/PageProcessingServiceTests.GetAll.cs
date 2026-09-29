// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        Page[] pages = [CreateRandomPage()];
        IQueryable<Page> queryablePages = pages.AsQueryable();

        pageServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: queryablePages);

        // When
        IQueryable<Page> result = pageProcessingService.GetAllPage();

        // Then
        result.Should()
            .BeSameAs(expected: queryablePages);

        pageServiceMock.Verify(expression: x => x.GetAllPage(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}