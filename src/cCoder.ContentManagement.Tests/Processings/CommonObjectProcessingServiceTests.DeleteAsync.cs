// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationWhenDeleteAsync()
    {
        commonObjectServiceMock
            .Setup(service => service.DeleteAsync(42))
            .Returns(ValueTask.CompletedTask);

        await commonObjectProcessingService.DeleteAsync(commonObjectId: 42);

        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}