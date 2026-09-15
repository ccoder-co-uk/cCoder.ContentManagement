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
        CommonObject commonObject = CreateRandomCommonObject(id: 9);

        commonObjectBrokerMock
            .Setup(broker => broker.GetAllCommonObjects())
            .Returns(new[] { commonObject }.AsQueryable());

        commonObjectBrokerMock
            .Setup(broker => broker.DeleteCommonObjectAsync(
                It.Is<CommonObject>(item => item.Id == commonObject.Id)))
            .ReturnsAsync(1);

        await commonObjectService.DeleteAsync(commonObjectId: commonObject.Id);

        commonObjectBrokerMock.VerifyAll();
        commonObjectBrokerMock.VerifyNoOtherCalls();
    }
}