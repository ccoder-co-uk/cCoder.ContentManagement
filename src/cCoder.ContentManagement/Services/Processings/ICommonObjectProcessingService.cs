using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Services.Processings;

public interface ICommonObjectProcessingService
{
    CommonObject Get(int id);

    IQueryable<CommonObject> GetAll(bool ignoreFilters = false);

    ValueTask<CommonObject> AddAsync(CommonObject entity);

    ValueTask<CommonObject> UpdateAsync(CommonObject entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<CommonObject>>> AddOrUpdate(IEnumerable<CommonObject> items);

    ValueTask DeleteAllAsync(IEnumerable<CommonObject> items);

    IEnumerable<CommonObject> Latest(string type);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<CommonObject>>> ImportAsync(IEnumerable<CommonObject> items);
}
