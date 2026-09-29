// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageInfoProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUserCanUpdatePageInfoForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        pageInfoServiceMock.Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: pageInfo))
            .ReturnsAsync(value: pageInfo);

        // When
        PageInfo result = await pageInfoProcessingService.UpdatePageInfoAsync(updatedPageInfo: pageInfo);

        // Then
        Assert.Same(expected: pageInfo, actual: result);
        pageInfoServiceMock.Verify(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: pageInfo), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        pageInfoServiceMock
            .Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: pageInfo))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await pageInfoProcessingService.UpdatePageInfoAsync(updatedPageInfo: pageInfo)
        );

        // Then
    }

}