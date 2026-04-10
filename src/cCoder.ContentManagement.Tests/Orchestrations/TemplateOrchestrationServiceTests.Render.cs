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

public partial class TemplateRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnRenderServiceResult()
    {
        object model = new();
        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = "en-GB",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = [],
        };
        templateRenderProcessingServiceMock
            .Setup(x =>
                x.Render(
                    1,
                    "template",
                    model,
                    It.IsAny<User>(),
                    "en-GB",
                    It.IsAny<cCoder.ContentManagement.Models.Config>(),
                    loggerMock.Object
                )
            )
            .Returns("rendered");

        string result = renderOrchestrationService.Render(1, "template", "en-GB", model, user);

        result.Should().Be("rendered");
        templateRenderProcessingServiceMock.VerifyAll();
    }

}




















