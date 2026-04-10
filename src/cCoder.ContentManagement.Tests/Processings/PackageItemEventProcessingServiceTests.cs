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
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageItemEventProcessingServiceTests
{
    private readonly Mock<IPackageItemEventService> packageItemEventServiceMock;
    private readonly PackageItemEventProcessingService service;

    public PackageItemEventProcessingServiceTests()
    {
        packageItemEventServiceMock = new Mock<IPackageItemEventService>(MockBehavior.Strict);
        service = new PackageItemEventProcessingService(packageItemEventServiceMock.Object);
    }

    private static PackageItem CreateRandomPackageItem() =>
        Builder<PackageItem>.CreateNew().Build();
}


















