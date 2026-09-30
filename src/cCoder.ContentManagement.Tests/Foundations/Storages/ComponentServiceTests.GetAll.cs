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

public partial class ComponentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Component[] expectedItems = [CreateRandomComponent()];

        IQueryable<CmsDataModels.Component> components = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        componentBrokerMock.Setup(expression: x => x.GetAllComponents())
            .Returns(value: components);

        // When
        IQueryable<Component> result = componentService.GetAllComponents();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        componentBrokerMock.Verify(expression: x => x.GetAllComponents(), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
    }

}