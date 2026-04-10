using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using System.Text.Json;
using cCoder.ContentManagement.Api.OData;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Exposures.Caching;
using Moq;
using cCoder.Data.Exposures;


namespace cCoder.Core.Services.Tests.CMS.Exposures.Caching;

public partial class MetadataCacheTests
{
    private readonly Mock<IMetadataTypeCache> metadataTypeCacheMock;
    private readonly Mock<cCoder.ContentManagement.Exposures.Caching.ICommonObjectCache> commonObjectCacheMock;

    public MetadataCacheTests()
    {
        metadataTypeCacheMock = new Mock<IMetadataTypeCache>(MockBehavior.Strict);
        commonObjectCacheMock = new Mock<cCoder.ContentManagement.Exposures.Caching.ICommonObjectCache>(MockBehavior.Strict);
    }

    private MetadataCache CreateSubject(params MetadataContainerSet[] typeSets)
    {
        metadataTypeCacheMock
            .Setup(cache => cache.GetAll())
            .Returns(typeSets.Select(static typeSet => JsonSerializer.Serialize(typeSet)).ToArray());

        commonObjectCacheMock
            .Setup(cache => cache.GetAll<Resource>())
            .Returns([]);

        return new MetadataCache(metadataTypeCacheMock.Object, commonObjectCacheMock.Object);
    }
}








