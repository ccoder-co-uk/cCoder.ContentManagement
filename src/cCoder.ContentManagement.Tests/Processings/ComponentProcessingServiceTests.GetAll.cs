// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Component> entities = new[] { CreateRandomComponent() }.AsQueryable();

        componentServiceMock.Setup(expression: x => x.GetAllComponent())
            .Returns(value: entities);

        // When
        IQueryable<Component> result = componentProcessingService.GetAllComponent();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        componentServiceMock.Verify(expression: x => x.GetAllComponent(), times: Times.Once);
        componentServiceMock.VerifyNoOtherCalls();
    }

}