// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

using DataCommonObject = cCoder.Data.Models.CommonObject;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;
namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    private readonly Mock<ICommonObjectBroker> commonObjectBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly CommonObjectService commonObjectService;

    public CommonObjectServiceTests()
    {
        commonObjectBrokerMock = new Mock<ICommonObjectBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        commonObjectService = new CommonObjectService(
commonObjectBroker: commonObjectBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static CommonObject CreateRandomCommonObject(int id = 42, int version = 1)
    {
        CommonObject commonObject = Builder<CommonObject>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.Version = version)
            .With(func: x => x.Key = $"key-{Guid.NewGuid():N}")
            .With(func: x => x.Type = "ContentManagement/Resource")
            .With(func: x => x.Json = "{}")
            .With(func: x => x.Culture = "en-GB")
            .With(func: x => x.Name = $"CommonObject-{Guid.NewGuid():N}")
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.LastUpdatedBy = "tester")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
            .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return commonObject;
    }

    private static DataCommonObject ToDataCommonObject(CommonObject commonObject) =>
        new()
        {
            Id = commonObject.Id,
            Name = commonObject.Name,
            Description = commonObject.Description,
            LastUpdated = commonObject.LastUpdated,
            LastUpdatedBy = commonObject.LastUpdatedBy,
            CreatedOn = commonObject.CreatedOn,
            CreatedBy = commonObject.CreatedBy,
            Version = commonObject.Version,
            Key = commonObject.Key,
            Type = commonObject.Type,
            Json = commonObject.Json,
            Culture = commonObject.Culture,
        };
}