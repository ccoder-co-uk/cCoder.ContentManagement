// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            Types = [new ExtendedMetadataContainer(type: typeof(string)) { Category = "Core" }],
        };

        MetadataContainerSet workflow = new()
        {
            Name = "Workflow",
            Types = [new ExtendedMetadataContainer(type: typeof(int)) { Category = "Workflow" }],
        };

        MetadataCache subject = CreateSubject(core, workflow);

        string result = subject.GetAll(culture: "en-GB");

        result.Should()
            .Contain(expected: "\"Name\":\"Core\"");

        result.Should()
            .Contain(expected: "\"Name\":\"Workflow\"");

        metadataTypeCacheMock.Verify(expression: cache => cache.GetAll(), times: Times.AtLeastOnce);

        commonObjectCacheMock.Verify(
expression: cache => cache.GetAll<Resource>(),
times: Times.Once
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
            Types = [new ExtendedMetadataContainer(type: typeof(string)) { Category = "Core" }],
        };

        MetadataContainerSet updated = new()
        {
            Name = "Workflow",
            Types = [new ExtendedMetadataContainer(type: typeof(int)) { Category = "Workflow" }],
        };

        string[] currentTypeSetPayloads = [JsonSerializer.Serialize(value: initial)];

        metadataTypeCacheMock
            .Setup(expression: cache => cache.GetAll())
            .Returns(valueFunction: () => currentTypeSetPayloads);

        commonObjectCacheMock
            .Setup(expression: cache => cache.GetAll<Resource>())
            .Returns(value: [])
            .Verifiable();

        MetadataCache subject = new(metadataTypeCache: metadataTypeCacheMock.Object, resourceCache: commonObjectCacheMock.Object);

        currentTypeSetPayloads = [JsonSerializer.Serialize(value: updated)];
        subject.Rebuild();
        string result = subject.GetAll(culture: "en-GB");

        result.Should()
            .Contain(expected: "\"Name\":\"Workflow\"");

        result.Should()
            .NotContain(unexpected: "\"Name\":\"Core\"");

        commonObjectCacheMock.Verify(
expression: cache => cache.GetAll<Resource>(),
times: Times.Exactly(callCount: 2)
        );

        metadataTypeCacheMock.Verify(expression: cache => cache.GetAll(), times: Times.AtLeast(callCount: 3));
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
            Types = [new ExtendedMetadataContainer(type: typeof(App)) { Category = "Core" }],
        };

        MetadataContainerSet appSecurity = new()
        {
            Name = "Core",
            UriBase = "Core",
            Types = [new ExtendedMetadataContainer(type: typeof(Role)) { Category = "Core" }],
        };

        MetadataCache subject = CreateSubject(contentManagement, appSecurity);

        string result = subject.GetAll(culture: "en-GB");

        result.Should()
            .Contain(expected: "\"Name\":\"Core\"");

        result.Should()
            .Contain(expected: "\"Name\":\"App\"");

        result.Should()
            .Contain(expected: "\"Name\":\"Role\"");

        subject.Get(key: "core/app", culture: "en-GB")
            .Should()
            .Contain(expected: "\"Name\":\"App\"");

        subject.Get(key: "core/role", culture: "en-GB")
            .Should()
            .Contain(expected: "\"Name\":\"Role\"");
    }
}