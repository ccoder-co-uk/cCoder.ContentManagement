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

public partial class PackageServiceTests
{
    private readonly Mock<IPackageBroker> packageBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly PackageService packageService;

    public PackageServiceTests()
    {
        packageBrokerMock = new Mock<IPackageBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        packageService = new PackageService(
            packageBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static Package CreateRandomPackage(Guid id = default)
    {
        Package package = Builder<Package>
            .CreateNew()
            .With(x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(x => x.Name = $"Package-{Guid.NewGuid():N}")
            .With(x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(x => x.Category = $"Category-{Guid.NewGuid():N}")
            .With(x => x.SourceApi = $"https://api-{Guid.NewGuid():N}.test")
            .With(x => x.Items = Array.Empty<PackageItem>())
            .Build();

        return package;
    }
}






















