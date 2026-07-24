// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task Get_GivenToolsPage_ShouldReturnManualTestingShell()
    {
        // Given
        string content = await GetOkContentAsync(path: "/tools/index.html");
        string apiScript = await GetOkContentAsync(path: "/tools/api.js");
        string gridScript = await GetOkContentAsync(path: "/tools/grids.js");
        // When
        string styles = await GetOkContentAsync(path: "/tools/styles.css");

        // Then
        content.Should()
            .Contain(expected: "Content Management");

        content.Should()
            .Contain(expected: "/tools/company-logo.png");

        content.Should()
            .Contain(expected: "cm-logo");

        content.Should()
            .Contain(expected: "Sign in required");

        content.Should()
            .Contain(expected: "cm-login-gate");

        content.Should()
            .Contain(expected: "cm-workbench");

        content.Should()
            .Contain(expected: "/tools/api.js");

        content.Should()
            .Contain(expected: "/tools/grids.js");

        content.Should()
            .Contain(expected: "auth-user");

        content.Should()
            .Contain(expected: "entity-nav");

        content.Should()
            .Contain(expected: "Content Management entity set tabs");

        content.Should()
            .Contain(expected: "entity-surfaces");

        apiScript.Should()
            .Contain(expected: "content-management-auth-changed");

        apiScript.Should()
            .Contain(expected: "isAuthenticated: function");

        gridScript.Should()
            .Contain(expected: "ContentManagementApi.isAuthenticated()");

        gridScript.Should()
            .Contain(expected: "content-management-auth-changed");

        gridScript.Should()
            .Contain(expected: "pageChildEntitySets");

        gridScript.Should()
            .Contain(expected: "pageDetailTemplate");

        gridScript.Should()
            .Contain(expected: "data-page-child-grid");

        gridScript.Should()
            .Contain(expected: "Page Info");

        gridScript.Should()
            .Contain(expected: "Page Roles");

        gridScript.Should()
            .NotContain(unexpected: "description: \"Metadata rows owned by the selected Page\"");

        gridScript.Should()
            .NotContain(unexpected: "description: \"Role links owned by the selected Page\"");

        styles.Should()
            .Contain(expected: "body.cm-shell:not(.is-authenticated) .cm-workbench");

        styles.Should()
            .Contain(expected: "body.cm-shell.is-authenticated .cm-login-gate");

        styles.Should()
            .Contain(expected: ".cm-logo");

        styles.Should()
            .Contain(expected: "grid-template-rows: auto minmax(0, 1fr)");

        styles.Should()
            .Contain(expected: ".cm-nav-item.active");
    }
}