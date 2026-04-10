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
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppCultureServiceTests
{
    private readonly Mock<IAppCultureBroker> appCultureBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly AppCultureService appCultureService;

    public AppCultureServiceTests()
    {
        appCultureBrokerMock = new Mock<IAppCultureBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        appCultureService = new AppCultureService(
            appCultureBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static AppCulture CreateRandomAppCulture(int appId = 1, string cultureId = null)
    {
        AppCulture appCulture = new()
        {
            AppId = appId,
            CultureId = cultureId ?? $"culture-{Guid.NewGuid():N}",
            App = null!,
            Culture = null!,
        };

        return appCulture;
    }
}





















