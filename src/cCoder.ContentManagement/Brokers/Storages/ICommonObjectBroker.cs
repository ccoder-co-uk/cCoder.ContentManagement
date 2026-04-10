using cCoder.Data.Models;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ICommonObjectBroker
{
    IQueryable<CommonObject> GetAllCommonObjects(bool ignoreFilters);

    CommonObject[] GetLatestCommonObjectsPaged(int pageSize = 500);

    ValueTask<CommonObject> AddCommonObjectAsync(CommonObject entity);

    ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject entity);

    ValueTask<int> DeleteCommonObjectAsync(CommonObject entity);

    ValueTask DeleteAllCommonObjectsAsync(IEnumerable<CommonObject> items);

    int? GetAppId(CommonObject entity);
}
