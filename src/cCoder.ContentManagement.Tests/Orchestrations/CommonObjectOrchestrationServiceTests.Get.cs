// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        CommonObject entity = CreateRandomCommonObject();

        commonObjectProcessingServiceMock.Setup(expression: x => x.GetCommonObject(commonObjectId: id))
            .Returns(value: entity);

        // When
        CommonObject result = orchestrationService.GetCommonObject(commonObjectId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        commonObjectProcessingServiceMock.Verify(expression: x => x.GetCommonObject(commonObjectId: id), times: Times.Once);
        commonObjectProcessingServiceMock.VerifyNoOtherCalls();
        authorizationProcessingServiceMock.VerifyNoOtherCalls();
    }

}