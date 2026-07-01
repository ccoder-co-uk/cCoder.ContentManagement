using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenHealthEndpoint_ShouldReturnOk()
    {
        string content = await GetOkContentAsync("/Health");

        content.Should().Be("OK");
    }
}
