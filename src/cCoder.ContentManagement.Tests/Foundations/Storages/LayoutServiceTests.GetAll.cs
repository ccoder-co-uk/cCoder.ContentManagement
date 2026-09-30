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

public partial class LayoutServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Layout[] expectedItems = [CreateRandomLayout()];

        IQueryable<CmsDataModels.Layout> layouts = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        layoutBrokerMock.Setup(expression: x => x.GetAllLayouts())
            .Returns(value: layouts);

        // When
        IQueryable<Layout> result = layoutService.GetAllLayouts();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        layoutBrokerMock.Verify(expression: x => x.GetAllLayouts(), times: Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
    }

}