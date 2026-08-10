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
        packageExportServiceMock = new Mock<IPackageExportService>(behavior: MockBehavior.Strict);
        processingService = new PackageExportProcessingService(packageExportService: packageExportServiceMock.Object);
    }

    private static Package CreatePackage(string name) =>
        new() { Name = name, Items = [] };

    private void SetupKnownPackageExport(int appId, string packageName, Package expectedPackage)
    {
        switch (packageName)
        {
            case "Roles":
                packageExportServiceMock.Setup(expression: x => x.ExportRolesPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
            case "Layouts":
                packageExportServiceMock.Setup(expression: x => x.ExportLayoutsPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
            case "Templates":
                packageExportServiceMock
                    .Setup(expression: x => x.ExportTemplatesPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
            case "Components":
                packageExportServiceMock
                    .Setup(expression: x => x.ExportComponentsPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
            case "Scripts":
                packageExportServiceMock.Setup(expression: x => x.ExportScriptsPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
            case "Resources":
                packageExportServiceMock
                    .Setup(expression: x => x.ExportResourcesPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
            case "Pages":
                packageExportServiceMock.Setup(expression: x => x.ExportPagesPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
            case "PageRoles":
                packageExportServiceMock
                    .Setup(expression: x => x.ExportPageRolesPackage(appId: appId))
                    .Returns(value: expectedPackage);
                break;
        }
    }
}