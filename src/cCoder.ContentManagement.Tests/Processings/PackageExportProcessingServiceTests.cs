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
using cCoder.ContentManagement.Services.Foundations.Exports;
using cCoder.ContentManagement.Services.Processings;
using Moq;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageExportProcessingServiceTests
{
    private readonly Mock<IPackageExportService> packageExportServiceMock;
    private readonly PackageExportProcessingService processingService;

    public PackageExportProcessingServiceTests()
    {
        packageExportServiceMock = new Mock<IPackageExportService>(MockBehavior.Strict);
        processingService = new PackageExportProcessingService(packageExportServiceMock.Object);
    }

    private static Package CreatePackage(string name) => new(name) { Items = [] };

    private void SetupKnownPackageExport(int appId, string packageName, Package expectedPackage)
    {
        switch (packageName)
        {
            case "Roles":
                packageExportServiceMock.Setup(x => x.ExportRoles(appId)).Returns(expectedPackage);
                break;
            case "Layouts":
                packageExportServiceMock.Setup(x => x.ExportLayouts(appId)).Returns(expectedPackage);
                break;
            case "Templates":
                packageExportServiceMock
                    .Setup(x => x.ExportTemplates(appId))
                    .Returns(expectedPackage);
                break;
            case "Components":
                packageExportServiceMock
                    .Setup(x => x.ExportComponents(appId))
                    .Returns(expectedPackage);
                break;
            case "Scripts":
                packageExportServiceMock.Setup(x => x.ExportScripts(appId)).Returns(expectedPackage);
                break;
            case "Resources":
                packageExportServiceMock
                    .Setup(x => x.ExportResources(appId))
                    .Returns(expectedPackage);
                break;
            case "Pages":
                packageExportServiceMock.Setup(x => x.ExportPages(appId)).Returns(expectedPackage);
                break;
            case "PageRoles":
                packageExportServiceMock
                    .Setup(x => x.ExportPageRoles(appId))
                    .Returns(expectedPackage);
                break;
        }
    }
}















