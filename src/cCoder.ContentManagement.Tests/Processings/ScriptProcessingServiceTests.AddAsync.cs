// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ScriptProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Script script = CreateRandomScript();

        scriptServiceMock.Setup(expression: x => x.AddScriptAsync(newScript: script))
            .ReturnsAsync(value: script);

        // When
        Script result = await scriptProcessingService.AddScriptAsync(newScript: script);

        // Then
        Assert.Same(expected: script, actual: result);
        scriptServiceMock.Verify(expression: x => x.AddScriptAsync(newScript: script), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Script script = CreateRandomScript();

        scriptServiceMock
            .Setup(expression: x => x.AddScriptAsync(newScript: script))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await scriptProcessingService.AddScriptAsync(newScript: script)
        );

        // Then
    }

}