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


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    private readonly Mock<IAppBroker> appBrokerMock;
    private readonly Mock<ICultureBroker> cultureBrokerMock;
    private readonly Mock<IPrivilegeBroker> privilegeBrokerMock;
    private readonly Mock<IPageBroker> pageBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly AppService appService;

    public AppServiceTests()
    {
        appBrokerMock = new Mock<IAppBroker>(MockBehavior.Strict);
        cultureBrokerMock = new Mock<ICultureBroker>(MockBehavior.Strict);
        privilegeBrokerMock = new Mock<IPrivilegeBroker>(MockBehavior.Strict);
        pageBrokerMock = new Mock<IPageBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        appService = new AppService(
            appBrokerMock.Object,
            pageBrokerMock.Object,
            authorizationBrokerMock.Object);
    }

    private static App CreateRandomApp(int id = 42)
    {
        App app = Builder<App>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.DefaultCultureId = "en-GB")
            .With(x => x.TenantId = $"tenant-{Guid.NewGuid():N}")
            .With(x => x.Name = $"App-{Guid.NewGuid():N}")
            .With(x => x.Domain = $"app-{Guid.NewGuid():N}.test")
            .With(x => x.DefaultTheme = "default")
            .With(x => x.ConfigJson = "{}")
            .Build();

        return app;
    }
}

























