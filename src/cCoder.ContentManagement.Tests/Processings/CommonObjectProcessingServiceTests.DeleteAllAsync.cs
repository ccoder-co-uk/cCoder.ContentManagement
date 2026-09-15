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
    public async Task ShouldDeleteEachItemWhenDeleteAllAsync()
    {
        CommonObject first = CreateRandomCommonObject();
        CommonObject second = CreateRandomCommonObject();

        commonObjectServiceMock.Setup(service => service.DeleteAsync(first.Id))
            .Returns(ValueTask.CompletedTask);
        commonObjectServiceMock.Setup(service => service.DeleteAsync(second.Id))
            .Returns(ValueTask.CompletedTask);

        await commonObjectProcessingService.DeleteAllCommonObjectAsync([first, second]);

        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}