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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceProcessingServiceTests
{
    [Fact]
    public async Task ShouldStampLastUpdatedFields_AndDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Resource resource = CreateRandomResource(appId: 7);
        User currentUser = TestUsers.WithPrivilege("resource_update");
        resourceServiceMock.Setup(x => x.UpdateAsync(resource)).ReturnsAsync(resource);

        // When
        Resource result = await resourceProcessingService.UpdateAsync(resource);

        // Then
        Assert.Same(resource, result);
        Assert.Equal("test-user", resource.LastUpdatedBy);
        resourceServiceMock.Verify(x => x.UpdateAsync(resource), Times.Once);
    }

}















