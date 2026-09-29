// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        CommonObject[] commonObjects = [CreateRandomCommonObject()];

        IQueryable<CommonObject> queryableCommonObjects =
            commonObjects.AsQueryable();

        commonObjectServiceMock.Setup(expression: x => x.GetAllCommonObject())
            .Returns(value: queryableCommonObjects);

        // When

        IQueryable<CommonObject> result =
            commonObjectProcessingService.GetAllCommonObject();

        // Then

        result.Should()
            .BeSameAs(expected: queryableCommonObjects);

        commonObjectServiceMock.Verify(expression: x => x.GetAllCommonObject(), times: Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
    }

}