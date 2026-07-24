// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class CommonObjectBroker(ICoreContextFactory coreContextFactory) : ICommonObjectBroker
{
    public IQueryable<CommonObject> GetAllCommonObjects(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.CommonObjects.IgnoreQueryFilters()
            : coreDataContext.CommonObjects;
    }

    public CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        int num = 0;
        List<CommonObject> list = new List<CommonObject>();

        while (true)
        {
            CommonObject[] array = coreDataContext.CommonObjects
                .AsNoTracking()
                .GroupBy(keySelector: commonObject => new { commonObject.Name, commonObject.Culture, commonObject.Key, commonObject.Type })
                .Select(selector: group => group.OrderByDescending(keySelector: version => version.Version)
                .First())
                .Skip(count: num)
                .Take(count: pageSize)
                .ToArray();

            if (array.Length == 0)
            {
                break;
            }

            list.AddRange(collection: array);
            num += pageSize;
        }

        return list.ToArray();
    }

    public async ValueTask<CommonObject> AddCommonObjectAsync(CommonObject entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CommonObject result = (await coreDataContext.CommonObjects.AddAsync(entity: entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        CommonObject result = coreDataContext.CommonObjects.Update(entity: entity)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCommonObjectAsync(CommonObject entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.CommonObjects.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCommonObjectsAsync(IEnumerable<CommonObject> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.CommonObjects.RemoveRange(entities: items);
        await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(CommonObject entity) =>
        null;
}