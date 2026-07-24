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

    public async ValueTask<Content> AddContentAsync(Content newContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Content result = (await coreDataContext.Contents.AddAsync(entity: newContent)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Content> UpdateContentAsync(Content updatedContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Content result = coreDataContext.Contents.Update(entity: updatedContent)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteContentAsync(Content deletedContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Contents.Remove(entity: deletedContent);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllContentsAsync(IEnumerable<Content> deletedContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Contents.RemoveRange(entities: deletedContent);
        await coreDataContext.SaveChangesAsync();
    }
}