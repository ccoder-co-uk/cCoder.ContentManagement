// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using System.Text.Json;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Models.OData;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Dependencies.Caching;
using Moq;
using cCoder.Data.Exposures;


namespace cCoder.Core.Services.Tests.CMS.Exposures.Caching;

public partial class MetadataCacheTests
{
    private readonly Mock<IMetadataTypeCache> metadataTypeCacheMock;
    private readonly Mock<cCoder.ContentManagement.Rendering.Brokers.ICommonObjectReaderBroker> commonObjectCacheMock;

    public MetadataCacheTests()
    {
        metadataTypeCacheMock = new Mock<IMetadataTypeCache>(behavior: MockBehavior.Strict);
        commonObjectCacheMock =
            new Mock<cCoder.ContentManagement.Rendering.Brokers.ICommonObjectReaderBroker>(
                behavior: MockBehavior.Strict);
    }

    private MetadataCacheDependency CreateSubject(params MetadataContainerSet[] typeSets)
    {
        metadataTypeCacheMock
            .Setup(expression: cache => cache.GetAll())
            .Returns(value: typeSets.Select(selector: static typeSet => JsonSerializer.Serialize(value: typeSet))
            .ToArray());

        commonObjectCacheMock
            .Setup(expression: cache => cache.GetAll<Resource>())
            .Returns(value: []);

        return new MetadataCacheDependency(
            metadataTypeCache: metadataTypeCacheMock.Object,
            resourceCache: commonObjectCacheMock.Object);
    }
}