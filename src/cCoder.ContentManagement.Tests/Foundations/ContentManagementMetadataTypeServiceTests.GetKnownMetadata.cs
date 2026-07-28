// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models.OData;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Extensions.OData;
using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations;

public partial class ContentManagementMetadataTypeServiceTests
{
    [Fact]
    public void ShouldReturnKnownMetadataSetsOnGetKnownMetadata()
    {
        // Given
        // When
        MetadataContainerSet[] result = service.GetKnownMetadata()
            .ToArray();

        // Then
        result.Select(selector: set => set.Name)
            .Should()
            .Equal(elements: ["ContentManagement", "System"]);
    }

    [Fact]
    public void ShouldReturnExpectedContentManagementTypesOnGetKnownMetadata()
    {
        // Given
        // When
        MetadataContainerSet result = service.GetKnownMetadata()
            .Single(predicate: set => set.Name == "ContentManagement");

        // Then
        result.UriBase.Should()
            .Be(expected: "ContentManagement");

        result.Types.Select(selector: type => type.Name)
            .Should()
            .Contain(expected: [
                nameof(App),
                nameof(Page),
                nameof(PageInfo),
                nameof(PageRole),
                nameof(Resource),
                nameof(RenderResult),
            ]);

        result.Types.Select(selector: type => type.Name)
            .Should()
            .NotContain(unexpected: [
                nameof(Package),
                nameof(User),
            ]);

        result.Types.Single(predicate: type => type.Name == nameof(App))
            .HasEndpoint.Should()
            .BeTrue();

        result.Types.Single(predicate: type => type.Name == nameof(App))
            .IsEntity.Should()
            .BeTrue();

        result.Types.Single(predicate: type => type.Name == nameof(RenderResult))
            .HasEndpoint.Should()
            .BeFalse();
    }
}