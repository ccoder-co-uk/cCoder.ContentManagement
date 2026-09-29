// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        Component entity = CreateRandomComponent();
        var id = entity.Id;

        componentServiceMock.Setup(expression: x => x.DeleteAsync(componentId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await componentProcessingService.DeleteAllComponentAsync(deletedComponent: new[] { entity });

        // Then
        componentServiceMock.Verify(expression: x => x.DeleteAsync(componentId: id), times: Times.Once);
        componentServiceMock.VerifyNoOtherCalls();
    }

}