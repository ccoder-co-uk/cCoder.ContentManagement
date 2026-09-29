// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Content entity = CreateRandomContent();
        var id = entity.Id;

        contentServiceMock.Setup(expression: x => x.DeleteAsync(contentId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await contentProcessingService.DeleteAsync(contentId: id);

        // Then
        contentServiceMock.Verify(expression: x => x.DeleteAsync(contentId: id), times: Times.Once);
        contentServiceMock.VerifyNoOtherCalls();
    }

}