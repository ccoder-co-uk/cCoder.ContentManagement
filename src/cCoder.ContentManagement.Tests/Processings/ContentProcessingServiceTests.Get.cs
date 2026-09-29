// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Content entity = CreateRandomContent();
        var id = entity.Id;

        contentServiceMock.Setup(expression: x => x.GetContent(contentId: id))
            .Returns(value: entity);

        // When
        Content result = contentProcessingService.GetContent(contentId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        contentServiceMock.Verify(expression: x => x.GetContent(contentId: id), times: Times.Once);
        contentServiceMock.VerifyNoOtherCalls();
    }

}