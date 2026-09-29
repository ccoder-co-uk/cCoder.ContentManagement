// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Page page = CreateRandomPage();

        pageServiceMock.Setup(expression: x => x.GetPage(pageId: page.Id))
            .Returns(value: page);

        // When
        Page result = pageProcessingService.GetPage(pageId: page.Id);

        // Then
        result.Should()
            .BeSameAs(expected: page);

        pageServiceMock.Verify(expression: x => x.GetPage(pageId: page.Id), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}