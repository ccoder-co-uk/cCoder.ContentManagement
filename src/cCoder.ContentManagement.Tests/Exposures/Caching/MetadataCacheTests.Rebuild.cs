using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using System.Text.Json;
using cCoder.ContentManagement.Api.OData;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Exposures.Caching;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Exposures.Caching;

public partial class MetadataCacheTests
{
    [Fact]
    public void ShouldSerializeAllKnownTypeSetsOnGetAll()
    {
        MetadataContainerSet core = new()
        {
            Name = "Core",
            Types = [new ExtendedMetadataContainer(typeof(string)) { Category = "Core" }],
        };
        MetadataContainerSet workflow = new()
        {
            Name = "Workflow",
            Types = [new ExtendedMetadataContainer(typeof(int)) { Category = "Workflow" }],
        };

        MetadataCache subject = CreateSubject(core, workflow);

        string result = subject.GetAll("en-GB");

        result.Should().Contain("\"Name\":\"Core\"");
        result.Should().Contain("\"Name\":\"Workflow\"");
        metadataTypeCacheMock.Verify(cache => cache.GetAll(), Times.AtLeastOnce);
        commonObjectCacheMock.Verify(
            cache => cache.GetAll<Resource>(),
            Times.Once
        );
        metadataTypeCacheMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRebuildUsingLatestSharedMetadataTypeSets()
    {
        MetadataContainerSet initial = new()
        {
            Name = "Core",
            Types = [new ExtendedMetadataContainer(typeof(string)) { Category = "Core" }],
        };
        MetadataContainerSet updated = new()
        {
            Name = "Workflow",
            Types = [new ExtendedMetadataContainer(typeof(int)) { Category = "Workflow" }],
        };

        string[] currentTypeSetPayloads = [JsonSerializer.Serialize(initial)];

        metadataTypeCacheMock
            .Setup(cache => cache.GetAll())
            .Returns(() => currentTypeSetPayloads);

        commonObjectCacheMock
            .Setup(cache => cache.GetAll<Resource>())
            .Returns([])
            .Verifiable();

        MetadataCache subject = new(metadataTypeCacheMock.Object, commonObjectCacheMock.Object);

        currentTypeSetPayloads = [JsonSerializer.Serialize(updated)];
        subject.Rebuild();
        string result = subject.GetAll("en-GB");

        result.Should().Contain("\"Name\":\"Workflow\"");
        result.Should().NotContain("\"Name\":\"Core\"");
        commonObjectCacheMock.Verify(
            cache => cache.GetAll<Resource>(),
            Times.Exactly(2)
        );
        metadataTypeCacheMock.Verify(cache => cache.GetAll(), Times.AtLeast(3));
        metadataTypeCacheMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldMergeTypeSetsWithTheSameNameOnGetAll()
    {
        MetadataContainerSet contentManagement = new()
        {
            Name = "Core",
            UriBase = "Core",
            Types = [new ExtendedMetadataContainer(typeof(App)) { Category = "Core" }],
        };
        MetadataContainerSet appSecurity = new()
        {
            Name = "Core",
            UriBase = "Core",
            Types = [new ExtendedMetadataContainer(typeof(Role)) { Category = "Core" }],
        };

        MetadataCache subject = CreateSubject(contentManagement, appSecurity);

        string result = subject.GetAll("en-GB");

        result.Should().Contain("\"Name\":\"Core\"");
        result.Should().Contain("\"Name\":\"App\"");
        result.Should().Contain("\"Name\":\"Role\"");
        subject.Get("core/app", "en-GB").Should().Contain("\"Name\":\"App\"");
        subject.Get("core/role", "en-GB").Should().Contain("\"Name\":\"Role\"");
    }
}







