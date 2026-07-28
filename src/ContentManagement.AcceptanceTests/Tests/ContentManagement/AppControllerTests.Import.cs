// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task ImportPackage_CreatesResourcesForApp()
    {
        // Given
        // When
        SeededApp app = await SeedDatabase(privileges: ["app_admin", "package_create", "resource_create", "resource_update"]);

        try
        {
            Package package = AcceptanceSeedData
                .LoadExportPackages()
                .First(predicate: found => string.Equals(a: found.Name, b: "Resources", comparisonType: StringComparison.OrdinalIgnoreCase));

            AppCmsChildren beforeImport = await GetAppCmsChildrenAsync(appId: app.AppId);
            int statusCode = await ImportPackageAsync(appId: app.AppId, package: package);
            AppCmsChildren afterImport = await GetAppCmsChildrenAsync(appId: app.AppId);

            // Then
            statusCode.Should()
                .Be(expected: (int)HttpStatusCode.OK);

            beforeImport.Resources.Should()
                .BeEmpty();

            afterImport.Resources.Should()
                .NotBeEmpty();
        }
        finally
        {
            await DeleteAppAsync(id: app.AppId);
        }
    }

    private async Task<int> ImportPackageAsync(int appId, Package package)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
requestUri: $"/Api/ContentManagement/Package/Import?appId={appId}",
value: package);

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }
}