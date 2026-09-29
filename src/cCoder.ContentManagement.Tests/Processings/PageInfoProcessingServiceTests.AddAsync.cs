// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();

        pageInfoServiceMock.Setup(expression: x => x.AddPageInfoAsync(newPageInfo: entity))
            .ReturnsAsync(value: entity);

        // When
        PageInfo result = await pageInfoProcessingService.AddPageInfoAsync(newPageInfo: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageInfoServiceMock.Verify(expression: x => x.AddPageInfoAsync(newPageInfo: entity), times: Times.Once);
        pageInfoServiceMock.VerifyNoOtherCalls();
    }

}