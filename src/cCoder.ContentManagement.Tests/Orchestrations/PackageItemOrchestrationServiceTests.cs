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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageItemOrchestrationServiceTests
{
    private readonly Mock<IPackageItemProcessingService> packageItemProcessingServiceMock;
    private readonly Mock<IPackageItemEventProcessingService> packageItemEventProcessingServiceMock;
    private readonly PackageItemOrchestrationService orchestrationService;
    public PackageItemOrchestrationServiceTests()
    {
        packageItemProcessingServiceMock = new Mock<IPackageItemProcessingService>(MockBehavior.Strict);
        packageItemEventProcessingServiceMock = new Mock<IPackageItemEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new PackageItemOrchestrationService(
            packageItemProcessingServiceMock.Object,
            packageItemEventProcessingServiceMock.Object
        );
    }
    private static PackageItem CreateRandomPackageItem() => Builder<PackageItem>.CreateNew().Build();
}





















