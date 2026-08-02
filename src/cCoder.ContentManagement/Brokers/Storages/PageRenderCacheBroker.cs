// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class PageRenderCacheBroker(ICoreContextFactory coreContextFactory) : IPageRenderCacheBroker
{
    public IQueryable<PageRenderCache> GetAllPageRenderCaches() =>
        coreContextFactory.CreateCoreContext().PageRenderCaches;

    public async ValueTask<PageRenderCache> AddPageRenderCacheAsync(PageRenderCache newPageRenderCache)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();
        PageRenderCache result = (await context.PageRenderCaches.AddAsync(entity: newPageRenderCache)).Entity;
        await context.SaveChangesAsync();
        return result;
    }

    public async ValueTask<PageRenderCache> UpdatePageRenderCacheAsync(PageRenderCache updatedPageRenderCache)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();
        PageRenderCache result = context.PageRenderCaches.Update(entity: updatedPageRenderCache).Entity;
        await context.SaveChangesAsync();
        return result;
    }

    public async ValueTask DeletePageRenderCacheAsync(PageRenderCache deletedPageRenderCache)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();
        context.PageRenderCaches.Remove(entity: deletedPageRenderCache);
        await context.SaveChangesAsync();
    }

    public async ValueTask ReplacePageRenderCachesByAppIdAsync(
        int appId,
        PageRenderCache[] replacements)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();

        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await context.Database.BeginTransactionAsync();

        PageRenderCache[] existing =
        [
            .. context.PageRenderCaches.Where(
                predicate: cache => cache.AppId == appId)
        ];

        context.PageRenderCaches.RemoveRange(entities: existing);
        await context.PageRenderCaches.AddRangeAsync(entities: replacements);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async ValueTask ReplacePageRenderCachesByPageIdsAsync(
        int appId,
        int[] pageIds,
        PageRenderCache[] replacements)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();

        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await context.Database.BeginTransactionAsync();

        PageRenderCache[] existing =
        [
            .. context.PageRenderCaches.Where(
                predicate: cache =>
                    cache.AppId == appId && pageIds.Contains(value: cache.PageId))
        ];

        context.PageRenderCaches.RemoveRange(entities: existing);
        await context.PageRenderCaches.AddRangeAsync(entities: replacements);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}