// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<CommonObject> entities = new[] { CreateRandomCommonObject() }.AsQueryable();

        commonObjectProcessingServiceMock.Setup(expression: x => x.GetAllCommonObjects(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<CommonObject> result = orchestrationService.GetAllCommonObjects(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        commonObjectProcessingServiceMock.Verify(expression: x => x.GetAllCommonObjects(ignoreFilters: true), times: Times.Once);
        commonObjectProcessingServiceMock.VerifyNoOtherCalls();
        authorizationProcessingServiceMock.VerifyNoOtherCalls();
    }

}