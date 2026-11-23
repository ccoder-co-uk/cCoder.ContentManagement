// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Services.Orchestrations.Caching;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.PageRendering;

public sealed partial class CommonObjectCacheRefreshPathTests
{
    [Fact]
    public void Refresh_WhenRequested_LoadsAndStoresOneSnapshot()
    {
        // Given
        CommonObjectCacheSnapshot snapshot = new();
        Mock<ICommonObjectCacheProcessingService> cacheProcessing = new();
        Mock<ICommonObjectLatestCacheProcessingService> sourceProcessing = new();

        sourceProcessing
            .Setup(expression: service =>
                service.LoadCommonObjectCacheSnapshot())
            .Returns(value: snapshot);

        cacheProcessing.Setup(expression: service =>
            service.SetCommonObjectCacheSnapshot(
                commonObjectCacheSnapshot: snapshot));

        CommonObjectCacheOrchestrationService service = new(
            cacheProcessingService: cacheProcessing.Object,
            latestCacheProcessingService: sourceProcessing.Object,
            jsonProcessingService: Mock.Of<IJsonProcessingService>());

        // When
        service.Refresh();

        // Then
        sourceProcessing.VerifyAll();
        cacheProcessing.VerifyAll();
    }

    [Fact]
    public void ApplyCommonObjects_WhenSnapshotExists_UpdatesOnlyChangedCacheEntry()
    {
        // Given
        CommonObject unchanged = new()
        {
            Id = 1,
            Type = "ContentManagement/Component",
            Name = "Unchanged"
        };

        CommonObject changed = new()
        {
            Id = 2,
            Type = "ContentManagement/Component",
            Name = "Changed",
            Json = "{\"Name\":\"Changed\"}"
        };

        CommonObjectCacheSnapshot current = new()
        {
            Items = new Dictionary<string, object>
            {
                ["component|unchanged"] = new Component { Name = "Unchanged" }
            },
            LatestSet = [unchanged]
        };

        Mock<ICommonObjectCacheProcessingService> cacheProcessing = new();
        Mock<ICommonObjectLatestCacheProcessingService> sourceProcessing = new();
        Mock<IJsonProcessingService> jsonProcessing = new();
        CommonObjectCacheSnapshot updated = null;

        cacheProcessing.Setup(expression: service =>
                service.GetCommonObjectCacheSnapshot())
            .Returns(value: current);

        cacheProcessing.Setup(expression: service =>
                service.SetCommonObjectCacheSnapshot(
                    commonObjectCacheSnapshot: It.IsAny<CommonObjectCacheSnapshot>()))
            .Callback<CommonObjectCacheSnapshot>(action: snapshot => updated = snapshot);

        jsonProcessing.Setup(expression: service => service.DeserializeItems<Component>(
                json: It.IsAny<string>()))
            .Returns(value: [new Component { Name = "Changed" }]);

        CommonObjectCacheOrchestrationService service = new(
            cacheProcessingService: cacheProcessing.Object,
            latestCacheProcessingService: sourceProcessing.Object,
            jsonProcessingService: jsonProcessing.Object);

        // When
        service.ApplyCommonObjects(changedCommonObjects: [changed]);

        // Then
        updated.LatestSet
            .Should()
            .BeEquivalentTo(expectation: [unchanged, changed]);

        updated.Items.Keys
            .Should()
            .BeEquivalentTo(expectation:
            [
                "component|unchanged",
                "component|changed"
            ]);

        sourceProcessing.Verify(
            expression: processing => processing.LoadCommonObjectCacheSnapshot(),
            times: Times.Never);
    }
}