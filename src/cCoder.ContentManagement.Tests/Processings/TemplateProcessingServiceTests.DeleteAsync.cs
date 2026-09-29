// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Template entity = CreateRandomTemplate();
        var id = entity.Id;

        templateServiceMock.Setup(expression: x => x.DeleteAsync(templateId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await templateProcessingService.DeleteAsync(templateId: id);

        // Then
        templateServiceMock.Verify(expression: x => x.DeleteAsync(templateId: id), times: Times.Once);
        templateServiceMock.VerifyNoOtherCalls();
    }

}