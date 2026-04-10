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
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;
using SecurityDataModels = cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ResourceServiceTests
{
    private readonly Mock<IResourceBroker> resourceBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly ResourceService resourceService;

    public ResourceServiceTests()
    {
        resourceBrokerMock = new Mock<IResourceBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        resourceBrokerMock = new();
        authorizationBrokerMock = new(MockBehavior.Strict);

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new SecurityDataModels.User { Id = "test-user" });

        resourceService = new ResourceService(
            resourceBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static Resource CreateRandomResource(int id = 42, int appId = 1, string key = null) =>
        Builder<Resource>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.AppId = appId)
            .With(x => x.Key = key ?? $"key-{Guid.NewGuid():N}")
            .With(x => x.Culture = "en-GB")
            .With(x => x.Name = $"Name-{Guid.NewGuid():N}")
            .With(x => x.DisplayName = $"Display-{Guid.NewGuid():N}")
            .With(x => x.ShortDisplayName = $"Short-{Guid.NewGuid():N}")
            .With(x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();
}





















