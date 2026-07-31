// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class CommonObjectBroker(ICoreContextFactory coreContextFactory) : ICommonObjectBroker
{
    public IQueryable<CommonObject> GetAllCommonObjects()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.CommonObjects;
    }

    public IQueryable<CommonObject> GetAllCommonObjectsIgnoringFilters()
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.CommonObjects.IgnoreQueryFilters();
    }

    public CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.CommonObjects
            .AsNoTracking()
            .GroupBy(keySelector: commonObject => new
            {
                commonObject.Name,
                Culture = commonObject.Culture ?? string.Empty,
                commonObject.Key,
                commonObject.Type
            })
            .Select(
                selector: group => group
                    .OrderByDescending(
                        keySelector: version => version.Version)
                    .First())
            .ToArray();
    }

    public async ValueTask<CommonObject> AddCommonObjectAsync(CommonObject newCommonObject)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CommonObject result = (await coreDataContext.CommonObjects.AddAsync(entity: newCommonObject)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        CommonObject result = coreDataContext.CommonObjects.Update(entity: updatedCommonObject)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCommonObjectAsync(CommonObject deletedCommonObject)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.CommonObjects.Remove(entity: deletedCommonObject);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCommonObjectsAsync(IEnumerable<CommonObject> deletedCommonObject)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.CommonObjects.RemoveRange(entities: deletedCommonObject);
        await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(CommonObject entity) =>
        null;
}