// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Caching;
using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task Post_ArrayRefreshesCommonObjectCacheAndClearsEveryPageRenderCache()
    {
        // Given
        string name = Unique(prefix: "ImportedComponent");
        string key = Unique(prefix: "key");
        string firstCacheId = $"1_91001__default-{Guid.NewGuid():N}";
        string secondCacheId = $"2_91002__default-{Guid.NewGuid():N}";
        DateTimeOffset originalTimestamp = DateTimeOffset.UtcNow.AddMinutes(minutes: -2);
        DateTimeOffset importedTimestamp = DateTimeOffset.UtcNow;
        const string type = "ContentManagement/Component";
        const string originalJson = "{\"Name\":\"Acceptance\",\"Content\":\"original-marker\",\"Script\":\"\"}";
        const string importedJson = "{\"Name\":\"Acceptance\",\"Content\":\"imported-marker\",\"Script\":\"\"}";

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using DbContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            core.Add(entity: new CommonObject
            {
                Name = name,
                Description = "Original acceptance component",
                LastUpdated = originalTimestamp,
                LastUpdatedBy = "Guest",
                CreatedOn = originalTimestamp,
                CreatedBy = "Guest",
                Version = 1,
                Key = key,
                Type = type,
                Json = originalJson,
                Culture = string.Empty
            });

            core.AddRange(
                new PageRenderCache
                {
                    Id = firstCacheId,
                    AppId = 1,
                    PageId = 91001,
                    Culture = string.Empty,
                    Theme = "default",
                    Path = "acceptance-one",
                    Header = "header-one",
                    Body = "body-one",
                    RenderedOn = originalTimestamp
                },
                new PageRenderCache
                {
                    Id = secondCacheId,
                    AppId = 2,
                    PageId = 91002,
                    Culture = string.Empty,
                    Theme = "default",
                    Path = "acceptance-two",
                    Header = "header-two",
                    Body = "body-two",
                    RenderedOn = originalTimestamp
                });

            await core.SaveChangesAsync();
        }

        ICommonObjectCache commonObjectCache = fixture.Factory.Services
            .GetRequiredService<ICommonObjectCache>();

        commonObjectCache.Refresh();

        // When
        await PostCommonObjectsAsync(payload: new
        {
            value = new object[]
            {
                new
                {
                    name,
                    description = "Imported acceptance component",
                    version = 1,
                    key,
                    type,
                    json = importedJson,
                    culture = string.Empty,
                    createdOn = importedTimestamp,
                    createdBy = "Guest",
                    lastUpdated = importedTimestamp,
                    lastUpdatedBy = "Guest"
                }
            }
        });

        CommonObject cachedObject = commonObjectCache.GetLatestSet()
            .Where(predicate: item =>
                item.Name == name
                && item.Key == key
                && item.Type == type
                && item.Culture == string.Empty)
            .OrderByDescending(keySelector: item => item.Version)
            .FirstOrDefault();

        CommonObject storedObject;
        PageRenderCache[] remainingCaches;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using DbContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            storedObject = await core.Set<CommonObject>()
                .IgnoreQueryFilters()
                .Where(predicate: item =>
                    item.Name == name
                    && item.Key == key
                    && item.Type == type
                    && item.Culture == string.Empty)
                .OrderByDescending(keySelector: item => item.Version)
                .FirstAsync();

            remainingCaches = await core.Set<PageRenderCache>()
                .Where(predicate: item =>
                    item.Id == firstCacheId || item.Id == secondCacheId)
                .ToArrayAsync();
        }

        // Then
        using AssertionScope assertionScope = new();

        storedObject.Json.Should()
            .Be(expected: importedJson, because: "the imported version must be persisted");

        cachedObject.Should()
            .NotBeNull(because: "the imported Common Object must remain available from the cache");

        cachedObject?.Json.Should()
            .Be(expected: importedJson, because: "the Common Object cache must expose the imported version");

        remainingCaches.Should()
            .BeEmpty(because: "a Common Cache import must invalidate rendered pages for every app");
    }
}
