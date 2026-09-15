// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    [Fact]
    public async Task ShouldLoadAndDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject(id: 9);

        commonObjectBrokerMock
            .Setup(expression: broker => broker.GetAllCommonObjects())
            .Returns(value: new[] { commonObject }.AsQueryable());

        commonObjectBrokerMock
            .Setup(expression: broker => broker.DeleteCommonObjectAsync(
                deletedCommonObject: It.Is<CommonObject>(match: item => item.Id == commonObject.Id)))
            .ReturnsAsync(value: 1);

        // When
        await commonObjectService.DeleteAsync(commonObjectId: commonObject.Id);

        // Then
        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}