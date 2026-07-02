using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenToolsPage_ShouldReturnManualTestingShell()
    {
        string content = await GetOkContentAsync("/tools/index.html");
        string apiScript = await GetOkContentAsync("/tools/api.js");
        string gridScript = await GetOkContentAsync("/tools/grids.js");
        string styles = await GetOkContentAsync("/tools/styles.css");

        content.Should().Contain("Content Management");
        content.Should().Contain("/tools/company-logo.png");
        content.Should().Contain("cm-logo");
        content.Should().Contain("Sign in required");
        content.Should().Contain("cm-login-gate");
        content.Should().Contain("cm-workbench");
        content.Should().Contain("/tools/api.js");
        content.Should().Contain("/tools/grids.js");
        content.Should().Contain("auth-user");
        content.Should().Contain("entity-nav");
        content.Should().Contain("Content Management entity set tabs");
        content.Should().Contain("entity-surfaces");
        apiScript.Should().Contain("content-management-auth-changed");
        apiScript.Should().Contain("isAuthenticated: function");
        gridScript.Should().Contain("ContentManagementApi.isAuthenticated()");
        gridScript.Should().Contain("content-management-auth-changed");
        gridScript.Should().Contain("pageChildEntitySets");
        gridScript.Should().Contain("pageDetailTemplate");
        gridScript.Should().Contain("data-page-child-grid");
        gridScript.Should().Contain("Page Info");
        gridScript.Should().Contain("Page Roles");
        gridScript.Should().NotContain("description: \"Metadata rows owned by the selected Page\"");
        gridScript.Should().NotContain("description: \"Role links owned by the selected Page\"");
        styles.Should().Contain("body.cm-shell:not(.is-authenticated) .cm-workbench");
        styles.Should().Contain("body.cm-shell.is-authenticated .cm-login-gate");
        styles.Should().Contain(".cm-logo");
        styles.Should().Contain("grid-template-rows: auto minmax(0, 1fr)");
        styles.Should().Contain(".cm-nav-item.active");
    }
}
