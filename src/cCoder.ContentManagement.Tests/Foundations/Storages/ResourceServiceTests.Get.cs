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
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    [Fact]
    public void ShouldReturnEntityWhenGet()
    {
        // Given

        // When
        Resource resource = CreateRandomResource(id: 5, appId: 7);
        resourceBrokerMock.Setup(x => x.GetAllResources(false)).Returns(new[] { resource }.AsQueryable());
        Resource result = resourceService.Get(5);

        // Then
        Assert.Equivalent(resource, result);
    }

}


















