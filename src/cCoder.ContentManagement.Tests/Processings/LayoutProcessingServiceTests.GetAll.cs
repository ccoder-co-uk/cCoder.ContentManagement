// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Layout> entities = new[] { CreateRandomLayout() }.AsQueryable();

        layoutServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: entities);

        // When
        IQueryable<Layout> result = layoutProcessingService.GetAllLayout();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        layoutServiceMock.Verify(expression: x => x.GetAllLayout(), times: Times.Once);
        layoutServiceMock.VerifyNoOtherCalls();
    }

}