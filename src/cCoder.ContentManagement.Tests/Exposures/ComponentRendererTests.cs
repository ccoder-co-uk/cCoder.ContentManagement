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

public class ComponentRendererTests
{
    [Fact]
    public void ShouldRenderThroughCoordinationService()
    {
        Mock<IComponentRenderCoordinationService> coordinationServiceMock = new(MockBehavior.Strict);
        ComponentRenderer renderer = new(coordinationServiceMock.Object);

        coordinationServiceMock
            .Setup(x => x.Render(1, "Hero", "en-GB", "Default"))
            .Returns("<section>hero</section>");

        string result = renderer.Render(1, "Hero", "en-GB", "Default");

        result.Should().Be("<section>hero</section>");
        coordinationServiceMock.VerifyAll();
    }
}


