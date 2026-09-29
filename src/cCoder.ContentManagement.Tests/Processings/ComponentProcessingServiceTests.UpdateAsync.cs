// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Component entity = CreateRandomComponent();

        componentServiceMock.Setup(expression: x => x.UpdateComponentAsync(updatedComponent: entity))
            .ReturnsAsync(value: entity);

        // When
        Component result = await componentProcessingService.UpdateComponentAsync(updatedComponent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        componentServiceMock.Verify(expression: x => x.UpdateComponentAsync(updatedComponent: entity), times: Times.Once);
        componentServiceMock.VerifyNoOtherCalls();
    }

}