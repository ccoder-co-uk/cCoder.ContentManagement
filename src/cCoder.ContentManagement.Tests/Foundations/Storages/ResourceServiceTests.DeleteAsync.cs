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
using CmsDataModels = cCoder.Data.Models.CMS;
using SecurityDataModels = cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    [Fact]
    public async Task ShouldDelegateToDataContextWhenUserHasDeletePrivilegeForDeleteAsync()
    {
        // Given

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SecurityDataModels.User { Id = "test-user" });

        Resource resource = CreateRandomResource(id: 5, appId: 7);
        resourceBrokerMock.Setup(x => x.GetAllResources(false)).Returns(new[] { resource }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Resource_delete"));
        resourceBrokerMock.Setup(x => x.DeleteResourceAsync(It.IsAny<CmsDataModels.Resource>())).ReturnsAsync(1);

        // When
        await resourceService.DeleteAsync(5);

        // Then
        resourceBrokerMock.Verify(x => x.DeleteResourceAsync(It.Is<CmsDataModels.Resource>(actual => actual.Id == resource.Id)), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SecurityDataModels.User { Id = "test-user" });

        Resource resource = CreateRandomResource(id: 5, appId: 7);
        resourceBrokerMock.Setup(x => x.GetAllResources(false)).Returns(new[] { resource }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Resource_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await resourceService.DeleteAsync(5)
        );

        // Then
    }

}
















