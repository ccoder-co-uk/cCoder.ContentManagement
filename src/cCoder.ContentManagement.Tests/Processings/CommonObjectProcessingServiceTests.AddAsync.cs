// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldNormalizeCultureAndDelegateToFoundationWhenAddAsync()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject();
        commonObject.Culture = null;

        commonObjectServiceMock
            .Setup(expression: service => service.AddCommonObjectAsync(
                newCommonObject: commonObject,
                userId: CurrentUserId))
            .ReturnsAsync(value: commonObject);

        // When
        CommonObject result = await commonObjectProcessingService.AddCommonObjectAsync(
            newCommonObject: commonObject,
            userId: CurrentUserId);

        // Then
        Assert.Same(expected: commonObject, actual: result);
        Assert.Equal(expected: string.Empty, actual: commonObject.Culture);
        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}