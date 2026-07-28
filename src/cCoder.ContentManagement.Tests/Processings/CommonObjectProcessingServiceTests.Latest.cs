// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using FluentAssertions;
using Xunit;

using DataCommonObject = cCoder.Data.Models.CommonObject;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public void ShouldFilterCacheByTypeWhenLatest()
    {
        // Given

        // When
        CommonObject expected = CreateRandomCommonObject(
type: "ContentManagement/Resource"
        );

        CommonObject other = CreateRandomCommonObject(
type: "ContentManagement/Component"
        );

        commonObjectCacheMock.Setup(expression: x => x.GetLatestSet())
            .Returns(
value: [
                new DataCommonObject
                {
                    Id = expected.Id,
                    Name = expected.Name,
                    Description = expected.Description,
                    LastUpdated = expected.LastUpdated,
                    LastUpdatedBy = expected.LastUpdatedBy,
                    CreatedOn = expected.CreatedOn,
                    CreatedBy = expected.CreatedBy,
                    Version = expected.Version,
                    Key = expected.Key,
                    Type = expected.Type,
                    Json = expected.Json,
                    Culture = expected.Culture,
                },
                new DataCommonObject
                {
                    Id = other.Id,
                    Name = other.Name,
                    Description = other.Description,
                    LastUpdated = other.LastUpdated,
                    LastUpdatedBy = other.LastUpdatedBy,
                    CreatedOn = other.CreatedOn,
                    CreatedBy = other.CreatedBy,
                    Version = other.Version,
                    Key = other.Key,
                    Type = other.Type,
                    Json = other.Json,
                    Culture = other.Culture,
                },
            ]
        );

        CommonObject[] results = commonObjectProcessingService
            .LatestCommonObject(type: "ContentManagement/Resource")
            .ToArray();

        // Then
        Assert.Single(collection: results);

        results[0].Should()
            .BeEquivalentTo(expectation: expected);
    }

}