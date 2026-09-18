// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Data;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

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

    public async ValueTask<PageRenderCache> StorePageRenderCacheAsync(
        PageRenderCache pageRenderCache)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();

        _ = await context.Database.ExecuteSqlInterpolatedAsync(
            sql: $"""
                MERGE [CMS].[PageRenderCache] WITH (HOLDLOCK) AS target
                USING (VALUES (
                    {pageRenderCache.Id},
                    {pageRenderCache.AppId},
                    {pageRenderCache.PageId},
                    {pageRenderCache.Culture},
                    {pageRenderCache.Theme},
                    {pageRenderCache.ParentId},
                    {pageRenderCache.Path},
                    {pageRenderCache.Title},
                    {pageRenderCache.Description},
                    {pageRenderCache.Keywords},
                    {pageRenderCache.ShowOnMenus},
                    {pageRenderCache.Header},
                    {pageRenderCache.Body},
                    {pageRenderCache.SourceFingerprint},
                    {pageRenderCache.RenderedOn}
                )) AS source (
                    [Id], [AppId], [PageId], [Culture], [Theme],
                    [ParentId], [Path], [Title], [Description], [Keywords],
                    [ShowOnMenus], [Header], [Body], [SourceFingerprint],
                    [RenderedOn]
                )
                ON target.[Id] = source.[Id]
                WHEN MATCHED THEN
                    UPDATE SET
                        target.[AppId] = source.[AppId],
                        target.[PageId] = source.[PageId],
                        target.[Culture] = source.[Culture],
                        target.[Theme] = source.[Theme],
                        target.[ParentId] = source.[ParentId],
                        target.[Path] = source.[Path],
                        target.[Title] = source.[Title],
                        target.[Description] = source.[Description],
                        target.[Keywords] = source.[Keywords],
                        target.[ShowOnMenus] = source.[ShowOnMenus],
                        target.[Header] = source.[Header],
                        target.[Body] = source.[Body],
                        target.[SourceFingerprint] = source.[SourceFingerprint],
                        target.[RenderedOn] = source.[RenderedOn]
                WHEN NOT MATCHED THEN
                    INSERT (
                        [Id], [AppId], [PageId], [Culture], [Theme],
                        [ParentId], [Path], [Title], [Description], [Keywords],
                        [ShowOnMenus], [Header], [Body], [SourceFingerprint],
                        [RenderedOn]
                    )
                    VALUES (
                        source.[Id], source.[AppId], source.[PageId],
                        source.[Culture], source.[Theme], source.[ParentId],
                        source.[Path], source.[Title], source.[Description],
                        source.[Keywords], source.[ShowOnMenus], source.[Header],
                        source.[Body], source.[SourceFingerprint],
                        source.[RenderedOn]
                    );
                """);

        return pageRenderCache;
    }

    public async ValueTask DeletePageRenderCacheAsync(string pageRenderCacheId)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();

        _ = await context.PageRenderCaches
            .Where(predicate: cache => cache.Id == pageRenderCacheId)
            .ExecuteDeleteAsync();
    }

    public async ValueTask ReplacePageRenderCachesByAppIdAsync(
        int appId,
        PageRenderCache[] replacements)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();

        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await context.Database.BeginTransactionAsync(
                isolationLevel: IsolationLevel.Serializable);

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
            await context.Database.BeginTransactionAsync(
                isolationLevel: IsolationLevel.Serializable);

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