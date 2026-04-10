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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    [Fact]
    public void ShouldReturnQueryWhenGetAll()
    {
        // Given

        // When
        Resource[] expectedItems =
        {
            CreateRandomResource(id: 1, appId: 7),
        };
        IQueryable<CmsDataModels.Resource> resources = expectedItems
            .Select(item => item)
            .AsQueryable();
        resourceBrokerMock.Setup(x => x.GetAllResources(false)).Returns(resources);
        IQueryable<Resource> result = resourceService.GetAll();

        // Then
        Assert.Single(result);
    }

}

















