using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenToolsPage_ShouldReturnManualTestingShell()
    {
        string content = await GetOkContentAsync("/tools/index.html");
        string gridScript = await GetOkContentAsync("/tools/grids.js");

        content.Should().Contain("Content Management");
        content.Should().Contain("/tools/api.js");
        content.Should().Contain("/tools/grids.js");
        content.Should().Contain("auth-user");
        content.Should().Contain("entity-nav");
        content.Should().Contain("entity-surfaces");
        gridScript.Should().Contain("pageChildEntitySets");
        gridScript.Should().Contain("pageDetailTemplate");
        gridScript.Should().Contain("data-page-child-grid");
        gridScript.Should().Contain("Page Info");
        gridScript.Should().Contain("Page Roles");
        gridScript.Should().NotContain("description: \"Metadata rows owned by the selected Page\"");
        gridScript.Should().NotContain("description: \"Role links owned by the selected Page\"");
    }
}
