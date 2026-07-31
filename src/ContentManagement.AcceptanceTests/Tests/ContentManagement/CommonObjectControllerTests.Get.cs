// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data;
using cCoder.ContentManagement.Exposures.Caching;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace Web.AcceptanceTests.Tests.ContentManagement;

public sealed partial class CommonObjectControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetCommonObjectCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCommonObjects()
    {
        // Given

        // When
        IReadOnlyList<CommonObject> actualCommonObjects = await GetCommonObjectsAsync();

        // Then

        actualCommonObjects.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsCommonObjectById()
    {
        // Given
        SeededCommonObjectContext seededContext = await SeedDatabase();
        CommonObject expectedCommonObject = new() { Id = seededContext.Id };

        // When
        CommonObject actualCommonObject = await GetCommonObjectAsync(id: seededContext.Id);

        // Then

        actualCommonObject.Should()
            .NotBeNull();

        actualCommonObject!.Id.Should()
            .Be(expected: expectedCommonObject.Id);

        await DeleteCommonObjectAsync(id: seededContext.Id);
        await Teardown(ids: seededContext.Id);
    }

    [Theory]
    [InlineData("ContentManagement/Resource")]
    [InlineData("ContentManagement/Component")]
    [InlineData("ContentManagement/Script")]
    public async Task Latest_ReturnsSeededCacheEntries(string type)
    {
        // Given

        // When
        IReadOnlyList<CommonObject> actualCommonObjects = await GetLatestCommonObjectsAsync(type: type);

        // Then

        actualCommonObjects.Count.Should()
            .BeGreaterThan(expected: 0);
    }

    [Fact]
    public async Task Latest_ReturnsOnlyHighestVersionWhenCultureIsNullOrEmpty()
    {
        // Given
        const string type = "ContentManagement/Component";
        string name = Unique(prefix: "CultureNormalisation");
        string key = Unique(prefix: "key");
        DateTimeOffset now = DateTimeOffset.UtcNow;
        int[] ids;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using CoreDataContext core = scope.ServiceProvider
                .GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            CommonObject[] commonObjects =
            [
                new()
                {
                    Name = name,
                    Description = "Older null-culture version",
                    LastUpdated = now,
                    LastUpdatedBy = "Guest",
                    CreatedOn = now,
                    CreatedBy = "Guest",
                    Version = 1,
                    Key = key,
                    Type = type,
                    Json = $"{{\"name\":\"{name}\",\"version\":1}}",
                    Culture = null
                },
                new()
                {
                    Name = name,
                    Description = "Latest empty-culture version",
                    LastUpdated = now,
                    LastUpdatedBy = "Guest",
                    CreatedOn = now,
                    CreatedBy = "Guest",
                    Version = 2,
                    Key = key,
                    Type = type,
                    Json = $"{{\"name\":\"{name}\",\"version\":2}}",
                    Culture = string.Empty
                }
            ];

            core.CommonObjects.AddRange(entities: commonObjects);
            await core.SaveChangesAsync();
            ids = commonObjects.Select(selector: commonObject => commonObject.Id).ToArray();
        }

        ICommonObjectCache cache = fixture.Factory.Services.GetRequiredService<ICommonObjectCache>();
        cache.Refresh();

        // When
        CommonObject[] actualCommonObjects = cache.GetLatestSet()
            .Where(predicate: commonObject => commonObject.Type == type
                && commonObject.Key == key
                && commonObject.Name == name)
            .ToArray();

        // Then
        actualCommonObjects.Should().ContainSingle();
        actualCommonObjects.Single().Version.Should().Be(expected: 2);

        await Teardown(ids: ids);
        cache.Refresh();
    }
}