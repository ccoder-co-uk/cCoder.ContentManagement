// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using System.Text.Json;
using cCoder.ContentManagement.Extensions.OData;
using cCoder.ContentManagement.Models.OData;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Dependencies.Caching;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Exposures.Caching;

public partial class MetadataCacheTests
{
    [Fact]
    public void ShouldSerializeAllKnownTypeSetsOnGetAll()
    {
        // Given
        MetadataContainerSet core = new()
        {
            Name = "Core",
            Types = [typeof(string).CreateExtendedMetadataContainer(category: "Core")],
        };

        MetadataContainerSet workflow = new()
        {
            Name = "Workflow",
            Types = [typeof(int).CreateExtendedMetadataContainer(category: "Workflow")],
        };

        MetadataCacheDependency subject = CreateSubject(typeSets: [core, workflow]);

        // When
        string result = subject.GetAll(culture: "en-GB");

        // Then
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
        // Given
        MetadataContainerSet initial = new()
        {
            Name = "Core",
            Types = [typeof(string).CreateExtendedMetadataContainer(category: "Core")],
        };

        MetadataContainerSet updated = new()
        {
            Name = "Workflow",
            Types = [typeof(int).CreateExtendedMetadataContainer(category: "Workflow")],
        };

        string[] currentTypeSetPayloads = [JsonSerializer.Serialize(value: initial)];

        metadataTypeCacheMock
            .Setup(expression: cache => cache.GetAll())
            .Returns(valueFunction: () => currentTypeSetPayloads);

        commonObjectCacheMock
            .Setup(expression: cache => cache.GetAll<Resource>())
            .Returns(value: [])
            .Verifiable();

        MetadataCacheDependency subject = new(
            metadataTypeCache: metadataTypeCacheMock.Object,
            resourceCache: commonObjectCacheMock.Object);

        currentTypeSetPayloads = [JsonSerializer.Serialize(value: updated)];
        subject.Rebuild();
        // When
        string result = subject.GetAll(culture: "en-GB");

        // Then
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
        // Given
        MetadataContainerSet contentManagement = new()
        {
            Name = "Core",
            UriBase = "Core",
            Types = [typeof(App).CreateExtendedMetadataContainer(category: "Core")],
        };

        MetadataContainerSet appSecurity = new()
        {
            Name = "Core",
            UriBase = "Core",
            Types = [typeof(Role).CreateExtendedMetadataContainer(category: "Core")],
        };

        MetadataCacheDependency subject = CreateSubject(typeSets: [contentManagement, appSecurity]);

        // When
        string result = subject.GetAll(culture: "en-GB");

        // Then
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