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
using Moq;
using IAuthorizationManager = cCoder.ContentManagement.Exposures.IAuthorizationManager;


using cCoder.ContentManagement.Exposures;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppCultureServiceTests
{
    private readonly Mock<IAppCultureBroker> appCultureBrokerMock;
    private readonly Mock<IAuthorizationManager> authorizationManagerMock;
    private readonly AppCultureService appCultureService;

    public AppCultureServiceTests()
    {
        appCultureBrokerMock = new Mock<IAppCultureBroker>(behavior: MockBehavior.Strict);
        authorizationManagerMock = new Mock<IAuthorizationManager>(behavior: MockBehavior.Strict);

        appCultureService = new AppCultureService(
appCultureBroker: appCultureBrokerMock.Object,
authorizationManager: authorizationManagerMock.Object
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