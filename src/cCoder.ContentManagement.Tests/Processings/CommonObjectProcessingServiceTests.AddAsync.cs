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
        CommonObject commonObject = CreateRandomCommonObject();
        commonObject.Culture = null;

        commonObjectServiceMock
            .Setup(service => service.AddCommonObjectAsync(commonObject, CurrentUserId))
            .ReturnsAsync(commonObject);

        CommonObject result = await commonObjectProcessingService.AddCommonObjectAsync(
            newCommonObject: commonObject,
            userId: CurrentUserId);

        Assert.Same(commonObject, result);
        Assert.Equal(string.Empty, commonObject.Culture);
        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}