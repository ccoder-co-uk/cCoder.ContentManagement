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

public partial class LayoutProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ILayoutService> layoutServiceMock = new();
    private readonly LayoutProcessingService layoutProcessingService;

    public LayoutProcessingServiceTests()
    {
        layoutProcessingService = new LayoutProcessingService(layoutServiceMock.Object);
    }

    private static Layout CreateRandomLayout() =>
        Builder<Layout>
            .CreateNew()
            .With(x => x.Id = Random.Shared.Next(1, 10000))
            .With(x => x.AppId = 1)
            .With(x => x.Name = $"Layout-{Guid.NewGuid():N}")
            .Build();
}


















