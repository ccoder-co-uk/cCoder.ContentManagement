// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Content content = CreateRandomContent();

        contentServiceMock.Setup(expression: x => x.AddContentAsync(newContent: content))
            .ReturnsAsync(value: content);

        // When
        Content result = await contentProcessingService.AddContentAsync(newContent: content);

        // Then
        Assert.Same(expected: content, actual: result);
        contentServiceMock.Verify(expression: x => x.AddContentAsync(newContent: content), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Content content = CreateRandomContent();

        contentServiceMock
            .Setup(expression: x => x.AddContentAsync(newContent: content))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await contentProcessingService.AddContentAsync(newContent: content)
        );

        // Then
    }

}