// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Culture entity = CreateRandomCulture();
        var id = entity.Id;

        cultureServiceMock.Setup(expression: x => x.DeleteAsync(cultureId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await cultureProcessingService.DeleteAsync(cultureId: id);

        // Then
        cultureServiceMock.Verify(expression: x => x.DeleteAsync(cultureId: id), times: Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}