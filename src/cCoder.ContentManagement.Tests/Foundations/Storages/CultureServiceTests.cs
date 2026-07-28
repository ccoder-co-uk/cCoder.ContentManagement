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
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    private readonly Mock<ICultureBroker> cultureBrokerMock;
    private readonly Mock<IAppCultureBroker> appCultureBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly CultureService cultureService;

    public CultureServiceTests()
    {
        cultureBrokerMock = new Mock<ICultureBroker>(behavior: MockBehavior.Strict);
        appCultureBrokerMock = new Mock<IAppCultureBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        cultureService = new CultureService(
cultureBroker: cultureBrokerMock.Object,
appCultureBroker: appCultureBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static Culture CreateRandomCulture(string id = null)
    {
        Culture culture = Builder<Culture>
            .CreateNew()
            .With(func: x => x.Id = id ?? $"culture-{Guid.NewGuid():N}")
            .With(func: x => x.Name = $"Culture-{Guid.NewGuid():N}")
            .Build();

        return culture;
    }
}