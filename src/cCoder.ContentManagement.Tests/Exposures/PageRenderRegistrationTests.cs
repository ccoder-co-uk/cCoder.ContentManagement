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
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public class PageRenderRegistrationTests
{
    [Fact]
    public void ShouldRegisterPageRendererAgainstNewRenderingCoordinationStack()
    {
        ServiceCollection services = [];

        services.AddContentManagement();

        ServiceDescriptor pageRendererDescriptor = services.Single(descriptor =>
            descriptor.ServiceType == typeof(IPageRenderer));

        pageRendererDescriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
        pageRendererDescriptor.ImplementationType.Should().NotBeNull();
        pageRendererDescriptor.ImplementationType!.FullName.Should().Be("cCoder.ContentManagement.Exposures.PageRenderer");

        Type coordinationServiceType = pageRendererDescriptor.ImplementationType
            .GetConstructors()
            .Single()
            .GetParameters()
            .Single()
            .ParameterType;

        coordinationServiceType.FullName.Should().Be(
            "cCoder.ContentManagement.Services.Coordinations.IPageRenderCoordinationService");
    }
}


