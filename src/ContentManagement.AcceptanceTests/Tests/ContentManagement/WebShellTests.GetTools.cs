using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenToolsPage_ShouldReturnManualTestingShell()
    {
        string content = await GetOkContentAsync("/tools/index.html");

        content.Should().Contain("Content Management");
        content.Should().Contain("/tools/api.js");
        content.Should().Contain("/tools/grids.js");
        content.Should().Contain("auth-user");
        content.Should().Contain("entity-nav");
        content.Should().Contain("entity-surfaces");
    }
}
