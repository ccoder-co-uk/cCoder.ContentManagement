// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

using DataCommonObject = cCoder.Data.Models.CommonObject;
namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject(id: 7);

        commonObjectBrokerMock
            .Setup(expression: x => x.GetAllCommonObjects())
            .Returns(value: new[] { ToDataCommonObject(commonObject: commonObject) }.AsQueryable());

        // When
        CommonObject result = commonObjectService.GetCommonObject(commonObjectId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: commonObject);

        commonObjectBrokerMock.Verify(expression: x => x.GetAllCommonObjects(), times: Times.Once);

        commonObjectBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<DataCommonObject>()),
times: Times.AtMostOnce()
        );

        commonObjectBrokerMock.VerifyNoOtherCalls();
    }

}