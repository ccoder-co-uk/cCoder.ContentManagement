// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Layout entity = CreateRandomLayout();
        var id = entity.Id;

        layoutServiceMock.Setup(expression: x => x.GetLayout(layoutId: id))
            .Returns(value: entity);

        // When
        Layout result = layoutProcessingService.GetLayout(layoutId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        layoutServiceMock.Verify(expression: x => x.GetLayout(layoutId: id), times: Times.Once);
        layoutServiceMock.VerifyNoOtherCalls();
    }

}