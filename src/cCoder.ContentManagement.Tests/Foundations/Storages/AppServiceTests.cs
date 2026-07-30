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
using IAuthorizationManager = cCoder.ContentManagement.Exposures.IAuthorizationManager;


using cCoder.ContentManagement.Exposures;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    private readonly Mock<IAppBroker> appBrokerMock;
    private readonly Mock<ICultureBroker> cultureBrokerMock;
    private readonly Mock<IPrivilegeBroker> privilegeBrokerMock;
    private readonly Mock<IAuthorizationManager> authorizationManagerMock;
    private readonly AppService appService;

    public AppServiceTests()
    {
        appBrokerMock = new Mock<IAppBroker>(behavior: MockBehavior.Strict);
        cultureBrokerMock = new Mock<ICultureBroker>(behavior: MockBehavior.Strict);
        privilegeBrokerMock = new Mock<IPrivilegeBroker>(behavior: MockBehavior.Strict);
        authorizationManagerMock = new Mock<IAuthorizationManager>(behavior: MockBehavior.Strict);

        appService = new AppService(
appBroker: appBrokerMock.Object,
authorizationManager: authorizationManagerMock.Object);
    }

    private static App CreateRandomApp(int id = 42)
    {
        App app = Builder<App>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.DefaultCultureId = "en-GB")
            .With(func: x => x.TenantId = $"tenant-{Guid.NewGuid():N}")
            .With(func: x => x.Name = $"App-{Guid.NewGuid():N}")
            .With(func: x => x.Domain = $"app-{Guid.NewGuid():N}.test")
            .With(func: x => x.DefaultTheme = "default")
            .With(func: x => x.ConfigJson = "{}")
            .Build();

        return app;
    }
}