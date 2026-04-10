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
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureProcessingServiceTests
{
    private readonly Mock<IAppCultureService> appCultureServiceMock = new();
    private readonly AppCultureProcessingService appCultureProcessingService;

    public AppCultureProcessingServiceTests()
    {
        appCultureProcessingService = new AppCultureProcessingService(appCultureServiceMock.Object);
    }

    private static AppCulture CreateRandomAppCulture() =>
        new()
        {
            AppId = 1,
            CultureId = $"culture-{Guid.NewGuid():N}",
            App = null!,
            Culture = null!,
        };
}





















