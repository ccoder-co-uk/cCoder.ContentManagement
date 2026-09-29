// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationWhenDeleteAsync()
    {
        // Given
        commonObjectServiceMock
            .Setup(expression: service => service.DeleteAsync(commonObjectId: 42))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await commonObjectProcessingService.DeleteAsync(commonObjectId: 42);

        // Then
        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}