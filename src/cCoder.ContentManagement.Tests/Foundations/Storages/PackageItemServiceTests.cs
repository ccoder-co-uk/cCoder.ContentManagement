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

public partial class PackageItemServiceTests
{
    private readonly Mock<IPackageItemBroker> packageItemBrokerMock;
    private readonly Mock<IAuthorizationManager> authorizationManagerMock;
    private readonly PackageItemService packageItemService;

    public PackageItemServiceTests()
    {
        packageItemBrokerMock = new Mock<IPackageItemBroker>(behavior: MockBehavior.Strict);
        authorizationManagerMock = new Mock<IAuthorizationManager>(behavior: MockBehavior.Strict);

        packageItemService = new PackageItemService(
packageItemBroker: packageItemBrokerMock.Object,
authorizationManager: authorizationManagerMock.Object
        );
    }

    private static PackageItem CreateRandomPackageItem(Guid id = default, Guid packageId = default)
    {
        PackageItem packageItem = Builder<PackageItem>
            .CreateNew()
            .With(func: x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(func: x => x.PackageId = packageId == Guid.Empty ? Guid.NewGuid() : packageId)
            .With(func: x => x.Type = $"Type-{Guid.NewGuid():N}")
            .With(func: x => x.Data = "{}")
            .Build();

        return packageItem;
    }
}