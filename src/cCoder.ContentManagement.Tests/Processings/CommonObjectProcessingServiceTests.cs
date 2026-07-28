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
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;

using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;
using JsonBroker = cCoder.ContentManagement.Brokers.JsonBroker;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    private readonly Mock<cCoder.ContentManagement.Rendering.Brokers.ICommonObjectReaderBroker> commonObjectCacheMock = new();
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ICommonObjectService> commonObjectServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly CommonObjectProcessingService commonObjectProcessingService;

    public CommonObjectProcessingServiceTests()
    {
        commonObjectProcessingService = new CommonObjectProcessingService(
service: commonObjectServiceMock.Object,
cache: commonObjectCacheMock.Object,
authorizationBroker: authorizationBrokerMock.Object,
jsonBroker: new JsonBroker()
        );
    }

    private static CommonObject CreateRandomCommonObject(
        string type = "ContentManagement/Resource"
    ) =>
        Builder<CommonObject>
            .CreateNew()
        .With(func: x => x.Id = Random.Shared.Next(minValue: 1, maxValue: 10000))
        .With(func: x => x.Name = $"CommonObject-{Guid.NewGuid():N}")
        .With(func: x => x.Key = $"key-{Guid.NewGuid():N}")
        .With(func: x => x.Culture = "en-GB")
        .With(func: x => x.Type = type)
        .With(func: x => x.Json = "{}")
        .With(func: x => x.Version = 1)
        .With(func: x => x.CreatedBy = "seed-user")
        .With(func: x => x.LastUpdatedBy = "seed-user")
        .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
        .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
        .Build();
}