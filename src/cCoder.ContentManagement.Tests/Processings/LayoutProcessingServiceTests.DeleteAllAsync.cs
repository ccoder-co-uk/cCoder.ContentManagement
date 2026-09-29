// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        Layout entity = CreateRandomLayout();
        var id = entity.Id;

        layoutServiceMock.Setup(expression: x => x.DeleteAsync(layoutId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await layoutProcessingService.DeleteAllLayoutAsync(deletedLayout: new[] { entity });

        // Then
        layoutServiceMock.Verify(expression: x => x.DeleteAsync(layoutId: id), times: Times.Once);
        layoutServiceMock.VerifyNoOtherCalls();
    }

}