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

public partial class PackageItemServiceTests
{
    private readonly Mock<IPackageItemBroker> packageItemBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly PackageItemService packageItemService;

    public PackageItemServiceTests()
    {
        packageItemBrokerMock = new Mock<IPackageItemBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        packageItemService = new PackageItemService(
            packageItemBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static PackageItem CreateRandomPackageItem(Guid id = default, Guid packageId = default)
    {
        PackageItem packageItem = Builder<PackageItem>
            .CreateNew()
            .With(x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(x => x.PackageId = packageId == Guid.Empty ? Guid.NewGuid() : packageId)
            .With(x => x.Type = $"Type-{Guid.NewGuid():N}")
            .With(x => x.Data = "{}")
            .Build();

        return packageItem;
    }
}






















