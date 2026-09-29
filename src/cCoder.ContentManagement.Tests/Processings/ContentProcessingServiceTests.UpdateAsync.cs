// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Content entity = CreateRandomContent();

        contentServiceMock.Setup(expression: x => x.UpdateContentAsync(updatedContent: entity))
            .ReturnsAsync(value: entity);

        // When
        Content result = await contentProcessingService.UpdateContentAsync(updatedContent: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        contentServiceMock.Verify(expression: x => x.UpdateContentAsync(updatedContent: entity), times: Times.Once);
        contentServiceMock.VerifyNoOtherCalls();
    }

}