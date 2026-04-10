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


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CultureProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ICultureService> cultureServiceMock = new();
    private readonly CultureProcessingService cultureProcessingService;

    public CultureProcessingServiceTests()
    {
        cultureProcessingService = new CultureProcessingService(cultureServiceMock.Object);
    }

    private static Culture CreateRandomCulture() =>
        Builder<Culture>
            .CreateNew()
            .With(x => x.Id = $"culture-{Guid.NewGuid():N}")
            .With(x => x.Name = $"Culture-{Guid.NewGuid():N}")
            .With(x => x.Apps = [])
            .With(x => x.Users = [])
            .With(x => x.PageInfos = [])
            .With(x => x.PageContents = [])
            .With(x => x.MetaItems = [])
            .Build();
}


















