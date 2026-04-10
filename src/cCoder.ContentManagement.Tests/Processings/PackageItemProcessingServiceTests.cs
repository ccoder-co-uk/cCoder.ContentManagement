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

public partial class PackageItemProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IPackageItemService> packageItemServiceMock = new();
    private readonly PackageItemProcessingService packageItemProcessingService;

    public PackageItemProcessingServiceTests()
    {
        packageItemProcessingService = new PackageItemProcessingService(packageItemServiceMock.Object);
    }

    private static PackageItem CreateRandomPackageItem() =>
        Builder<PackageItem>
            .CreateNew()
            .With(x => x.Type = $"Type-{Guid.NewGuid():N}")
            .With(x => x.Data = $"Data-{Guid.NewGuid():N}")
            .With(x => x.Package = null)
            .Build();
}




















