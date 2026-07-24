// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class ContentBroker(ICoreContextFactory coreContextFactory) : IContentBroker
{
    public IQueryable<Content> GetAllContents(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Contents.IgnoreQueryFilters()
            : coreDataContext.Contents;
    }

    public async ValueTask<Content> AddContentAsync(Content entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Content result = (await coreDataContext.Contents.AddAsync(entity: entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Content> UpdateContentAsync(Content entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Content result = coreDataContext.Contents.Update(entity: entity)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteContentAsync(Content entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Contents.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllContentsAsync(IEnumerable<Content> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Contents.RemoveRange(entities: items);
        await coreDataContext.SaveChangesAsync();
    }
}