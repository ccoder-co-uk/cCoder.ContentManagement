// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();

        layoutServiceMock.Setup(expression: x => x.UpdateLayoutAsync(updatedLayout: entity))
            .ReturnsAsync(value: entity);

        // When
        Layout result = await layoutProcessingService.UpdateLayoutAsync(updatedLayout: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        layoutServiceMock.Verify(expression: x => x.UpdateLayoutAsync(updatedLayout: entity), times: Times.Once);
        layoutServiceMock.VerifyNoOtherCalls();
    }

}