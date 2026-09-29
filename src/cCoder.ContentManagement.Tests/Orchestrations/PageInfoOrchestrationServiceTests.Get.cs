// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        PageInfo entity = CreateRandomPageInfo();

        pageInfoProcessingServiceMock.Setup(expression: x => x.GetPageInfo(pageInfoId: id))
            .Returns(value: entity);

        // When
        PageInfo result = orchestrationService.GetPageInfo(pageInfoId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageInfoProcessingServiceMock.Verify(expression: x => x.GetPageInfo(pageInfoId: id), times: Times.Once);
        pageInfoProcessingServiceMock.VerifyNoOtherCalls();
        pageInfoEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}