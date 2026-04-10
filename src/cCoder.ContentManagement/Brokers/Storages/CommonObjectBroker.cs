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
            CommonObject[] array = (from c in coreDataContext.CommonObjects.AsNoTracking()
                                    group c by new { c.Name, c.Culture, c.Key, c.Type } into c
                                    select c.OrderByDescending((CommonObject v) => v.Version).First()).Skip(num).Take(pageSize).ToArray();
            if (array.Length == 0)
            {
                break;
            }
            list.AddRange(array);
            num += pageSize;
        }
        return list.ToArray();
    }

    public async ValueTask<CommonObject> AddCommonObjectAsync(CommonObject entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CommonObject result = (await coreDataContext.CommonObjects.AddAsync(entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        CommonObject result = coreDataContext.CommonObjects.Update(entity).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteCommonObjectAsync(CommonObject entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.CommonObjects.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllCommonObjectsAsync(IEnumerable<CommonObject> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.CommonObjects.RemoveRange(items);
        await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(CommonObject entity)
    {
        return null;
    }
}
