// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ComponentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Component component = CreateRandomComponent(id: 7);

        componentBrokerMock.Setup(expression: x => x.GetAllComponents())
            .Returns(value: new[] { component }.AsQueryable());

        // When
        Component result = componentService.GetComponent(componentId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: component);

        componentBrokerMock.Verify(expression: x => x.GetAllComponents(), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
    }

}