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
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Coordinations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public class TemplateRendererTests
{
    [Fact]
    public void ShouldRenderThroughCoordinationService()
    {
        Mock<ITemplateRenderCoordinationService> coordinationServiceMock = new(MockBehavior.Strict);
        TemplateRenderer renderer = new(coordinationServiceMock.Object);
        object model = new { Name = "Ward" };

        coordinationServiceMock
            .Setup(x => x.Render(1, "Welcome", "en-GB", model))
            .Returns("<main>welcome</main>");

        string result = renderer.Render(1, "Welcome", "en-GB", model);

        result.Should().Be("<main>welcome</main>");
        coordinationServiceMock.VerifyAll();
    }
}


