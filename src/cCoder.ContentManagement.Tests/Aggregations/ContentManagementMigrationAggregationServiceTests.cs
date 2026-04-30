using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Moq;
using Xunit;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Tests.Aggregations;

public class ContentManagementMigrationAggregationServiceTests
{
    [Fact]
    public async Task ImportPackageAsync_ShouldIgnoreComputedAuditFields_WhenImportingComponents()
    {
        Mock<IComponentOrchestrationService> componentOrchestrationServiceMock = new();
        Component[] importedComponents = null;

        componentOrchestrationServiceMock
            .Setup(service => service.ImportComponentsAsync(It.IsAny<int>(), It.IsAny<Component[]>()))
            .Callback<int, Component[]>((_, items) => importedComponents = items)
            .Returns(ValueTask.CompletedTask);

        ContentManagementMigrationAggregationService service = CreateService(
            componentOrchestrationService: componentOrchestrationServiceMock.Object);

        Package package = new()
        {
            Items =
            [
                new PackageItem
                {
                    Type = "Core/Component",
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

        await service.ImportPackageAsync(1, package);

        importedComponents.Should().NotBeNull();
        importedComponents.Should().ContainSingle();
        importedComponents![0].Name.Should().Be("DetailedNav");
    }

    [Fact]
    public async Task ImportPackageAsync_ShouldIgnoreComputedAuditFields_WhenImportingPageArrays()
    {
        Mock<IPageOrchestrationService> pageOrchestrationServiceMock = new();
        cCoder.Data.Models.CMS.Page[] importedPages = null;

        pageOrchestrationServiceMock
            .Setup(service => service.ImportPagesAsync(It.IsAny<int>(), It.IsAny<cCoder.Data.Models.CMS.Page[]>()))
            .Callback<int, cCoder.Data.Models.CMS.Page[]>((_, items) => importedPages = items)
            .Returns(ValueTask.CompletedTask);

        ContentManagementMigrationAggregationService service = CreateService(
            pageOrchestrationService: pageOrchestrationServiceMock.Object);

        Package package = new()
        {
            Items =
            [
                new PackageItem
                {
                    Type = "Core/Page",
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

        await service.ImportPackageAsync(1, package);

        importedPages.Should().NotBeNull();
        importedPages.Should().ContainSingle();
        importedPages![0].Name.Should().Be("Home");
        importedPages[0].PageInfo.Should().ContainSingle();
        importedPages[0].PageInfo.First().Title.Should().Be("Home");
    }

    private static ContentManagementMigrationAggregationService CreateService(
        IComponentOrchestrationService componentOrchestrationService = null,
        ILayoutOrchestrationService layoutOrchestrationService = null,
        IPageOrchestrationService pageOrchestrationService = null,
        IPageRoleOrchestrationService pageRoleOrchestrationService = null,
        IResourceOrchestrationService resourceOrchestrationService = null,
        ITemplateOrchestrationService templateOrchestrationService = null,
        IScriptOrchestrationService scriptOrchestrationService = null)
        =>
        new(
            new JsonBroker(),
            componentOrchestrationService ?? Mock.Of<IComponentOrchestrationService>(),
            layoutOrchestrationService ?? Mock.Of<ILayoutOrchestrationService>(),
            pageOrchestrationService ?? Mock.Of<IPageOrchestrationService>(),
            pageRoleOrchestrationService ?? Mock.Of<IPageRoleOrchestrationService>(),
            resourceOrchestrationService ?? Mock.Of<IResourceOrchestrationService>(),
            templateOrchestrationService ?? Mock.Of<ITemplateOrchestrationService>(),
            scriptOrchestrationService ?? Mock.Of<IScriptOrchestrationService>());
}
