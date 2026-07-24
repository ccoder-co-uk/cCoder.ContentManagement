// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
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

        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await scriptProcessingService.AddScriptAsync(newScript: script)
        );

        // Then
    }

}