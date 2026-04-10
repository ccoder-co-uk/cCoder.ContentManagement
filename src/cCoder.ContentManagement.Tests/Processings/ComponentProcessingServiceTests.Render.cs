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
            .Setup(x => x.Render(app.Id, "Hero", actor, string.Empty, "Default"))
            .Returns("<section name='Hero' class='component'><div>content</div><script>console.log('component');</script></section>");

        // When
        string result = renderOrchestrationService.Render(app.Id, "Hero", actor, string.Empty, "Default");

        // Then
        result.Should().Contain("<section name='Hero' class='component'");
        result.Should().Contain("<div>content</div>");
        result.Should().Contain("console.log('component');");
        componentRenderProcessingServiceMock.Verify(x => x.Render(app.Id, "Hero", actor, string.Empty, "Default"), Times.Once);
    }

}






















