// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class LayoutServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Layout layout = CreateRandomLayout(id: 7);

        layoutBrokerMock.Setup(expression: x => x.GetAllLayouts())
            .Returns(value: new[] { layout }.AsQueryable());

        // When
        Layout result = layoutService.GetLayout(layoutId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: layout);

        layoutBrokerMock.Verify(expression: x => x.GetAllLayouts(), times: Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
    }

}