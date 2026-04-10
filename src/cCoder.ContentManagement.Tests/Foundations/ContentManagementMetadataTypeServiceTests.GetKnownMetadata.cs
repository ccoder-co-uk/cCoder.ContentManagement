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
using cCoder.ContentManagement.Api.OData;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations;

public partial class ContentManagementMetadataTypeServiceTests
{
    [Fact]
    public void ShouldReturnKnownMetadataSetsOnGetKnownMetadata()
    {
        MetadataContainerSet[] result = service.GetKnownMetadata().ToArray();

        result.Select(set => set.Name).Should().Equal("Core", "System");
    }

    [Fact]
    public void ShouldReturnExpectedCoreTypesOnGetKnownMetadata()
    {
        MetadataContainerSet result = service.GetKnownMetadata().Single(set => set.Name == "Core");

        result.UriBase.Should().Be("Core");
        result.Types.Select(type => type.Name)
            .Should()
            .Contain([
                nameof(App),
                nameof(Page),
                nameof(PageInfo),
                nameof(PageRole),
                nameof(Resource),
                nameof(RenderResult),
            ]);

        result.Types.Select(type => type.Name)
            .Should()
            .NotContain([
                nameof(Package),
                nameof(User),
            ]);

        result.Types.Single(type => type.Name == nameof(App)).HasEndpoint.Should().BeTrue();
        result.Types.Single(type => type.Name == nameof(App)).IsEntity.Should().BeTrue();
        result.Types.Single(type => type.Name == nameof(RenderResult)).HasEndpoint.Should().BeFalse();
    }
}







