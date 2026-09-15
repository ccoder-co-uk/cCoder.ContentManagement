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
        // Given
        CommonObject first = CreateRandomCommonObject();
        CommonObject second = CreateRandomCommonObject();

        commonObjectServiceMock
            .Setup(expression: service => service.DeleteAsync(commonObjectId: first.Id))
            .Returns(value: ValueTask.CompletedTask);

        commonObjectServiceMock
            .Setup(expression: service => service.DeleteAsync(commonObjectId: second.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await commonObjectProcessingService.DeleteAllCommonObjectAsync(
            deletedCommonObject: [first, second]);

        // Then
        commonObjectServiceMock.VerifyAll();
        VerifyNoOtherCommonObjectServiceCalls();
    }
}