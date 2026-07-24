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
        scriptServiceMock.Setup(x => x.AddScriptAsync(script)).ReturnsAsync(script);

        // When
        Script result = await scriptProcessingService.AddScriptAsync(script);

        // Then
        Assert.Same(script, result);
        scriptServiceMock.Verify(x => x.AddScriptAsync(script), Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Script script = CreateRandomScript();

        scriptServiceMock
            .Setup(x => x.AddScriptAsync(script))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await scriptProcessingService.AddScriptAsync(script)
        );

        // Then
    }

}














