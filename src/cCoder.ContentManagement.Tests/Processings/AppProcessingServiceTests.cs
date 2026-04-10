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
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IAppService> appServiceMock = new();
    private readonly Mock<ICultureService> cultureServiceMock = new();
    private readonly Mock<IPrivilegeBroker> privilegeBrokerMock = new();
    private readonly Mock<IAppEventProcessingService> appEventProcessingServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly AppProcessingService appProcessingService;

    public AppProcessingServiceTests()
    {
        appProcessingService = new AppProcessingService(
            appServiceMock.Object,
            cultureServiceMock.Object,
            privilegeBrokerMock.Object,
            appEventProcessingServiceMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static App CreateRandomApp() =>
        Builder<App>
            .CreateNew()
            .With(x => x.Id = Random.Shared.Next(1, 10000))
            .With(x => x.DefaultCultureId = string.Empty)
            .With(x => x.Name = $"App-{Guid.NewGuid():N}")
            .With(x => x.Domain = $"{Guid.NewGuid():N}.local")
            .With(x => x.DefaultTheme = "Default")
            .With(x => x.ConfigJson = "{}")
            .With(x => x.Cultures = [])
            .With(x => x.Pages = [])
            .With(x => x.Components = [])
            .With(x => x.Scripts = [])
            .With(x => x.Roles = [])
            .With(x => x.Templates = [])
            .With(x => x.Resources = [])
            .With(x => x.Layouts = [])
            .Build();
}


















