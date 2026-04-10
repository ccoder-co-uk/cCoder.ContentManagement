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
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;

using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;
using JsonBroker = cCoder.ContentManagement.Brokers.JsonBroker;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    private readonly Mock<cCoder.ContentManagement.Exposures.Caching.ICommonObjectCache> commonObjectCacheMock = new();
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ICommonObjectService> commonObjectServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly CommonObjectProcessingService commonObjectProcessingService;

    public CommonObjectProcessingServiceTests()
    {
        commonObjectProcessingService = new CommonObjectProcessingService(
            commonObjectServiceMock.Object,
            commonObjectCacheMock.Object,
            authorizationBrokerMock.Object,
            new JsonBroker()
        );
    }

    private static CommonObject CreateRandomCommonObject(
        string type = "Core/Resource"
    ) =>
        Builder<CommonObject>
            .CreateNew()
            .With(x => x.Id = Random.Shared.Next(1, 10000))
            .With(x => x.Name = $"CommonObject-{Guid.NewGuid():N}")
            .With(x => x.Key = $"key-{Guid.NewGuid():N}")
            .With(x => x.Culture = "en-GB")
            .With(x => x.Type = type)
            .With(x => x.Json = "{}")
            .With(x => x.Version = 1)
            .With(x => x.CreatedBy = "seed-user")
            .With(x => x.LastUpdatedBy = "seed-user")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5))
            .With(x => x.LastUpdated = DateTimeOffset.UtcNow.AddMinutes(-5))
            .Build();
}


























