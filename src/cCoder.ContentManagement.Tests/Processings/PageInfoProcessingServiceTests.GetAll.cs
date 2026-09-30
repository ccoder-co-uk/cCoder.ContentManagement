// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<PageInfo> entities = new[] { CreateRandomPageInfo() }.AsQueryable();

        pageInfoServiceMock.Setup(expression: x => x.GetAllPageInfos())
            .Returns(value: entities);

        // When
        IQueryable<PageInfo> result = pageInfoProcessingService.GetAllPageInfos();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        pageInfoServiceMock.Verify(expression: x => x.GetAllPageInfos(), times: Times.Once);
        pageInfoServiceMock.VerifyNoOtherCalls();
    }

}