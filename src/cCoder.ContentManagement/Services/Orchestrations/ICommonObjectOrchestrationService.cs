using cCoder.Data.Models;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ICommonObjectOrchestrationService
{
    CommonObject Get(int id);

    IQueryable<CommonObject> GetAll(bool ignoreFilters = false);

    ValueTask<CommonObject> AddAsync(CommonObject entity);

    ValueTask<CommonObject> UpdateAsync(CommonObject entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<CommonObject>>> AddOrUpdate(IEnumerable<CommonObject> items);

    ValueTask DeleteAllAsync(IEnumerable<CommonObject> items);

    IEnumerable<CommonObject> Latest(string type);

    ValueTask<IEnumerable<Result<CommonObject>>> ImportAsync(IEnumerable<CommonObject> items);
}
