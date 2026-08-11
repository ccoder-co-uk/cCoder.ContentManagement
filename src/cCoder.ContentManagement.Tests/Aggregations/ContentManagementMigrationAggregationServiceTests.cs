// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Services.Foundations.Serialization;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Moq;
using Xunit;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Tests.Aggregations;

public partial class ContentManagementMigrationAggregationServiceTests
{
    [Fact]
    public async Task ImportPackageAsync_ShouldIgnoreComputedAuditFields_WhenImportingComponents()
    {
        // Given
        Mock<IComponentOrchestrationService> componentOrchestrationServiceMock = new();
        Component[] importedComponents = null;

        componentOrchestrationServiceMock
            .Setup(expression: service => service.ImportComponentsAsync(appId: It.IsAny<int>(), items: It.IsAny<Component[]>()))
            .Callback<int, Component[]>(action: (_, items) => importedComponents = items)
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementMigrationAggregationService service = CreateService(
componentOrchestrationService: componentOrchestrationServiceMock.Object);

        Package package = new()
        {
            Items =
            [
                new PackageItem
                {
                    Type = "ContentManagement/Component",
                    Data = """
                           {
                             "Name": "DetailedNav",
                             "Description": "Navigation",
                             "LastUpdated": "",
                             "LastUpdatedBy": "",
                             "CreatedOn": "",
                             "CreatedBy": "",
                             "Content": "<nav></nav>",
                             "Script": "console.log('nav');",
                             "Key": "detailed-nav"
                           }
                           """
                }
            ]
        };

        // When
        await service.ImportPackageAsync(appId: 1, package: package);

        // Then
        importedComponents.Should()
            .NotBeNull();

        importedComponents.Should()
            .ContainSingle();

        importedComponents![0].Name.Should()
            .Be(expected: "DetailedNav");
    }

    [Fact]
    public async Task ImportPackageAsync_ShouldIgnoreComputedAuditFields_WhenImportingPageArrays()
    {
        // Given
        Mock<IPageOrchestrationService> pageOrchestrationServiceMock = new();
        cCoder.Data.Models.CMS.Page[] importedPages = null;

        pageOrchestrationServiceMock
            .Setup(expression: service => service.ImportPagesAsync(appId: It.IsAny<int>(), items: It.IsAny<cCoder.Data.Models.CMS.Page[]>()))
            .Callback<int, cCoder.Data.Models.CMS.Page[]>(action: (_, items) => importedPages = items)
            .ReturnsAsync(value: Array.Empty<cCoder.Data.Models.CMS.Page>());

        ContentManagementMigrationAggregationService service = CreateService(
            pageOrchestrationService: pageOrchestrationServiceMock.Object);

        Package package = new()
        {
            Items =
            [
                new PackageItem
                {
                    Type = "ContentManagement/Page",
                    Data = """
                           [
                             {
                               "Name": "Home",
                               "Path": "",
                               "Layout": "PublicSite",
                               "LastUpdated": "",
                               "CreatedOn": "",
                               "PageInfo": [
                                 {
                                   "Title": "Home",
                                   "LastUpdated": "",
                                   "CreatedOn": ""
                                 }
                               ]
                             }
                           ]
                           """
                }
            ]
        };

        // When
        await service.ImportPackageAsync(appId: 1, package: package);

        // Then
        importedPages.Should()
            .NotBeNull();

        importedPages.Should()
            .ContainSingle();

        importedPages![0].Name.Should()
            .Be(expected: "Home");

        importedPages[0].PageInfo.Should()
            .ContainSingle();

        importedPages[0].PageInfo.First()
            .Title.Should()
            .Be(expected: "Home");
    }

    private static ContentManagementMigrationAggregationService CreateService(
        IPackageExportProcessingService packageExportProcessingService = null,
        IComponentOrchestrationService componentOrchestrationService = null,
        ILayoutOrchestrationService layoutOrchestrationService = null,
        IPageOrchestrationService pageOrchestrationService = null,
        IPageImportOrchestrationService pageImportOrchestrationService = null,
        IResourceOrchestrationService resourceOrchestrationService = null,
        ITemplateOrchestrationService templateOrchestrationService = null,
        IScriptOrchestrationService scriptOrchestrationService = null,
        ICommonObjectOrchestrationService commonObjectOrchestrationService = null)
        =>
        new(
migrationSupportOrchestrationService: new MigrationSupportOrchestrationService(
jsonProcessingService: new JsonProcessingService(
jsonService: new JsonService(jsonBroker: new JsonBroker())),
packageExportProcessingService: packageExportProcessingService ?? Mock.Of<IPackageExportProcessingService>()),
componentOrchestrationService: componentOrchestrationService ?? Mock.Of<IComponentOrchestrationService>(),
layoutOrchestrationService: layoutOrchestrationService ?? Mock.Of<ILayoutOrchestrationService>(),
pageOrchestrationService: pageOrchestrationService ?? Mock.Of<IPageOrchestrationService>(),
pageImportOrchestrationService: pageImportOrchestrationService ?? Mock.Of<IPageImportOrchestrationService>(),
resourceOrchestrationService: resourceOrchestrationService ?? Mock.Of<IResourceOrchestrationService>(),
templateOrchestrationService: templateOrchestrationService ?? Mock.Of<ITemplateOrchestrationService>(),
scriptOrchestrationService: scriptOrchestrationService ?? Mock.Of<IScriptOrchestrationService>(),
commonObjectOrchestrationService: commonObjectOrchestrationService ?? Mock.Of<ICommonObjectOrchestrationService>(),
pageRenderCacheImportState: new cCoder.ContentManagement.Models.PageRenderCacheImportState());
}