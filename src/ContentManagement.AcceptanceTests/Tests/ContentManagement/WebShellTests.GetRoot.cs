using System.Net;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenRoot_ShouldRedirectToManualTestingShell()
    {
        using HttpResponseMessage response = await Client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.Should().Be("/tools/index.html");
    }
}
