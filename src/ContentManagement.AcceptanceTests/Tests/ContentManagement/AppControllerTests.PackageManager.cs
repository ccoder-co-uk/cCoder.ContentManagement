// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task PackageManager_ShouldRoundTripAppContentBetweenAppsAsync()
    {
        // Given
        string[] privileges =
        [
            "app_admin",
            "component_create",
            "component_update",
            "layout_create",
            "layout_update",
            "page_create",
            "page_update",
            "resource_create",
            "resource_update",
            "script_create",
            "script_update",
            "template_create",
            "template_update"
        ];

        SeededApp sourceApp = await SeedDatabase(privileges: privileges);
        SeededApp targetApp = await SeedDatabase(privileges: privileges);

        try
        {
            await using AsyncServiceScope scope =
                fixture.Factory.Services.CreateAsyncScope();

            IContentManagementPackageManager packageManager =
                scope.ServiceProvider.GetRequiredService<
                    IContentManagementPackageManager>();

            await packageManager.ImportPackageAsync(
                appId: sourceApp.AppId,
                package: CreateRepresentativeContentManagementPackage());

            ImportGraph expected = await GetImportGraphAsync(
                appId: sourceApp.AppId);

            string[] packageNames =
            [
                "Components",
                "Layouts",
                "Pages",
                "Resources",
                "Scripts",
                "Templates"
            ];

            Package[] exportedPackages = packageNames
                .Select(selector: packageName => packageManager.ExportPackage(
                    appId: sourceApp.AppId,
                    packageName: packageName))
                .ToArray();

            // When
            foreach (Package package in exportedPackages)
            {
                await packageManager.ImportPackageAsync(
                    appId: targetApp.AppId,
                    package: package);
            }

            ImportGraph actual = await GetImportGraphAsync(
                appId: targetApp.AppId);

            // Then
            exportedPackages.Should()
                .OnlyContain(predicate: package => package.Items.Count == 1);

            actual.Should()
                .BeEquivalentTo(expectation: expected);
        }
        finally
        {
            await DeleteAppAsync(id: targetApp.AppId);
            await DeleteAppAsync(id: sourceApp.AppId);
        }
    }
}
