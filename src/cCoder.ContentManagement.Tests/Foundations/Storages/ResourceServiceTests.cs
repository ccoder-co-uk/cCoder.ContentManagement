// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    private readonly Mock<IResourceBroker> resourceBrokerMock;
    private readonly ResourceService resourceService;

    public ResourceServiceTests()
    {
        resourceBrokerMock = new Mock<IResourceBroker>(behavior: MockBehavior.Strict);
        resourceService = new ResourceService(resourceBroker: resourceBrokerMock.Object);
    }

    private static Resource CreateRandomResource(int id = 42, int appId = 1, string key = null) =>
        Builder<Resource>
            .CreateNew()
        .With(func: x => x.Id = id)
        .With(func: x => x.AppId = appId)
        .With(func: x => x.Key = key ?? $"key-{Guid.NewGuid():N}")
        .With(func: x => x.Culture = "en-GB")
        .With(func: x => x.Name = $"Name-{Guid.NewGuid():N}")
        .With(func: x => x.DisplayName = $"Display-{Guid.NewGuid():N}")
        .With(func: x => x.ShortDisplayName = $"Short-{Guid.NewGuid():N}")
        .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
        .Build();
}
