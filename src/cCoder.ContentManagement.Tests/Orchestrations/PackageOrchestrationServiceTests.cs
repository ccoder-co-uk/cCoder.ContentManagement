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
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PackageOrchestrationServiceTests
{
    private readonly Mock<IContentManagementMigrationAggregationService> contentManagementMigrationAggregationServiceMock;
    private readonly Mock<IPackageExportProcessingService> packageExportProcessingServiceMock;
    private readonly Mock<IPackageProcessingService> packageProcessingServiceMock;
    private readonly Mock<IPackageEventProcessingService> packageEventProcessingServiceMock;
    private readonly PackageOrchestrationService orchestrationService;

    public PackageOrchestrationServiceTests()
    {
        contentManagementMigrationAggregationServiceMock = new Mock<IContentManagementMigrationAggregationService>(MockBehavior.Strict);
        packageExportProcessingServiceMock = new Mock<IPackageExportProcessingService>(MockBehavior.Strict);
        packageProcessingServiceMock = new Mock<IPackageProcessingService>(MockBehavior.Strict);
        packageEventProcessingServiceMock = new Mock<IPackageEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new PackageOrchestrationService(
            contentManagementMigrationAggregationServiceMock.Object,
            packageExportProcessingServiceMock.Object,
            packageProcessingServiceMock.Object,
            packageEventProcessingServiceMock.Object
        );
    }

    private static Package CreateRandomPackage() => Builder<Package>.CreateNew().Build();
}





















