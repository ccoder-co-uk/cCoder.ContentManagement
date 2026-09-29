// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Layout layout = CreateRandomLayout();

        layoutServiceMock.Setup(expression: x => x.AddLayoutAsync(newLayout: layout))
            .ReturnsAsync(value: layout);

        // When
        Layout result = await layoutProcessingService.AddLayoutAsync(newLayout: layout);

        // Then
        Assert.Same(expected: layout, actual: result);
        layoutServiceMock.Verify(expression: x => x.AddLayoutAsync(newLayout: layout), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Layout layout = CreateRandomLayout();

        layoutServiceMock
            .Setup(expression: x => x.AddLayoutAsync(newLayout: layout))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await layoutProcessingService.AddLayoutAsync(newLayout: layout)
        );

        // Then
    }

}