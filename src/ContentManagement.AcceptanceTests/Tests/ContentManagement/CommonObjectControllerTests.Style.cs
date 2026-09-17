// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Caching;
using cCoder.Data.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task StyleShouldFlowFromApiIntoCommonCacheProjection()
    {
        // Given
        string name = Unique(prefix: "Style");
        string key = Unique(prefix: "key");

        Style style = new()
        {
            Name = name,
            Description = "Acceptance style",
            Key = key,
            Content = ".acceptance-style { display: block; }"
        };

        CommonObject createdCommonObject = await CreateCommonObjectAsync(
            payload: new
            {
                name,
                description = style.Description,
                version = 1,
                key,
                type = "ContentManagement/Style",
                json = JsonSerializer.Serialize(value: style),
                culture = string.Empty,
            });

        ICommonObjectCache cache = fixture.Factory.Services
            .GetRequiredService<ICommonObjectCache>();

        cache.Refresh();

        ICacheBroker cacheBroker = fixture.Factory.Services
            .GetRequiredService<ICacheBroker>();

        // When
        CommonObjectCacheSnapshot snapshot = cacheBroker
            .Get<CommonObjectCacheSnapshot>(
                key: "ContentManagement.CommonObjects");

        Style actualStyle = snapshot.Items.Values
            .OfType<Style>()
            .Single(predicate: item => item.Name == name);

        IReadOnlyList<CommonObject> latestStyles =
            await GetLatestCommonObjectsAsync(
                type: "ContentManagement/Style");

        // Then
        actualStyle.Content.Should()
            .Be(expected: style.Content);

        latestStyles.Should()
            .ContainSingle(predicate: commonObject =>
                commonObject.Id == createdCommonObject.Id);

        await DeleteCommonObjectAsync(id: createdCommonObject.Id);
        await Teardown(ids: createdCommonObject.Id);
        cache.Refresh();
    }
}