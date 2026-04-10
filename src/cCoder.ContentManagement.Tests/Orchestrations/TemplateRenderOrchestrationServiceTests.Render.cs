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
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldRenderTemplateThroughProcessingService()
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
        object model = new { Name = "Ward" };
        string expectedHtml = "<main>template</main>";

        templateRenderProcessingServiceMock
            .Setup(x => x.Render(
                1,
                "Welcome",
                model,
                user,
                "en-GB",
                It.IsAny<Config>(),
                It.IsAny<ILogger>()))
            .Returns(expectedHtml);

        string result = renderOrchestrationService.Render(1, "Welcome", "en-GB", model, user);

        result.Should().Be(expectedHtml);
        templateRenderProcessingServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenUserIsNull()
    {
        Action act = () => renderOrchestrationService.Render(1, "Welcome", "en-GB", new { }, null!);

        act.Should().Throw<ValidationException>().WithMessage("user is required.");
    }
}




