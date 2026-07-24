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

public partial class ComponentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Component component = CreateRandomComponent();
        componentServiceMock.Setup(x => x.AddComponentAsync(component)).ReturnsAsync(component);

        // When
        Component result = await componentProcessingService.AddComponentAsync(component);

        // Then
        Assert.Same(component, result);
        componentServiceMock.Verify(x => x.AddComponentAsync(component), Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Component component = CreateRandomComponent();

        componentServiceMock
            .Setup(x => x.AddComponentAsync(component))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await componentProcessingService.AddComponentAsync(component)
        );

        // Then
    }

}














