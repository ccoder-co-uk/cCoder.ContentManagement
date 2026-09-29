// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();
        var id = entity.Id;

        pageInfoServiceMock.Setup(expression: x => x.GetPageInfo(pageInfoId: id))
            .Returns(value: entity);

        // When
        PageInfo result = pageInfoProcessingService.GetPageInfo(pageInfoId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageInfoServiceMock.Verify(expression: x => x.GetPageInfo(pageInfoId: id), times: Times.Once);
        pageInfoServiceMock.VerifyNoOtherCalls();
    }

}