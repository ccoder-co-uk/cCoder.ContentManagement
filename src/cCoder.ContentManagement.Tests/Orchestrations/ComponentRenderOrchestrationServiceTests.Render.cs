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
using System.ComponentModel.DataAnnotations;



using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ComponentRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldRenderComponentThroughProcessingService()
    {
        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = "en-GB",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = []
        };
        string expectedHtml = "<section>component</section>";

        componentRenderProcessingServiceMock
            .Setup(x => x.Render(1, "Hero", user, "en-GB", "Default"))
            .Returns(expectedHtml);

        string result = renderOrchestrationService.Render(1, "Hero", user, "en-GB", "Default");

        result.Should().Be(expectedHtml);
        componentRenderProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenUserIsNull()
    {
        Action act = () => renderOrchestrationService.Render(1, "Hero", null!, "en-GB", "Default");

        act.Should().Throw<ValidationException>().WithMessage("user is required.");
    }
}




