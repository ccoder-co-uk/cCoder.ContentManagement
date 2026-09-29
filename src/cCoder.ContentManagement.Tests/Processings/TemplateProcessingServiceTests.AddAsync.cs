// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Template template = CreateRandomTemplate();

        templateServiceMock.Setup(expression: x => x.AddTemplateAsync(newTemplate: template))
            .ReturnsAsync(value: template);

        // When
        Template result = await templateProcessingService.AddTemplateAsync(newTemplate: template);

        // Then
        Assert.Same(expected: template, actual: result);
        templateServiceMock.Verify(expression: x => x.AddTemplateAsync(newTemplate: template), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Template template = CreateRandomTemplate();

        templateServiceMock
            .Setup(expression: x => x.AddTemplateAsync(newTemplate: template))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await templateProcessingService.AddTemplateAsync(newTemplate: template)
        );

        // Then
    }

}