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
        SeededApp app = await SeedDatabase("app_admin", "package_create", "resource_create", "resource_update");

        try
        {
            Package package = AcceptanceSeedData
                .LoadExportPackages()
                .First(found => string.Equals(found.Name, "Resources", StringComparison.OrdinalIgnoreCase));

            AppCmsChildren beforeImport = await GetAppCmsChildrenAsync(app.AppId);
            int statusCode = await ImportPackageAsync(app.AppId, package);
            AppCmsChildren afterImport = await GetAppCmsChildrenAsync(app.AppId);

            statusCode.Should().Be((int)HttpStatusCode.OK);
            beforeImport.Resources.Should().BeEmpty();
            afterImport.Resources.Should().NotBeEmpty();
        }
        finally
        {
            await DeleteAppAsync(app.AppId);
        }
    }

    private async Task<int> ImportPackageAsync(int appId, Package package)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/Api/Core/Package/Import?appId={appId}",
            package);

        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }
}

