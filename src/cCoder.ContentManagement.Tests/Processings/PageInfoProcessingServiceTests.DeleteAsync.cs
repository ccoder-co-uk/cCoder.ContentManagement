// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        PageInfo entity = CreateRandomPageInfo();
        var id = entity.Id;

        pageInfoServiceMock.Setup(expression: x => x.DeleteAsync(pageInfoId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await pageInfoProcessingService.DeleteAsync(pageInfoId: id);

        // Then
        pageInfoServiceMock.Verify(expression: x => x.DeleteAsync(pageInfoId: id), times: Times.Once);
        pageInfoServiceMock.VerifyNoOtherCalls();
    }

}