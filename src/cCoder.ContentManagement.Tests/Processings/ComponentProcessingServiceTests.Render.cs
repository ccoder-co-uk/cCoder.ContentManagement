// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldRenderComponentMarkupWhenComponentExistsInApp()
    {
        // Given
        User actor = new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = []
        };

        Component component = CreateRandomComponent();
        component.Name = "Hero";
        component.Content = "<div>content</div>";
        component.Script = "console.log('component');";

        App app = new()
        {
            Id = 1,
        };

        componentRenderProcessingServiceMock
            .Setup(expression: x => x.RenderUser(appId: app.Id, name: "Hero", user: actor, culture: string.Empty, theme: "Default"))
            .Returns(value: "<section name='Hero' class='component'><div>content</div><script>console.log('component');</script></section>");

        // When
        string result = renderOrchestrationService.RenderUser(appId: app.Id, name: "Hero", user: actor, culture: string.Empty, theme: "Default");

        // Then

        result.Should()
            .Contain(expected: "<section name='Hero' class='component'");

        result.Should()
            .Contain(expected: "<div>content</div>");

        result.Should()
            .Contain(expected: "console.log('component');");

        componentRenderProcessingServiceMock.Verify(expression: x => x.RenderUser(appId: app.Id, name: "Hero", user: actor, culture: string.Empty, theme: "Default"), times: Times.Once);
    }

}