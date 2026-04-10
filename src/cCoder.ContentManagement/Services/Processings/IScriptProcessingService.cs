using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Processings;

public interface IScriptProcessingService
{
    Script Get(int id);

    IQueryable<Script> GetAll(bool ignoreFilters = false);

    ValueTask<Script> AddAsync(Script entity);

    ValueTask<Script> UpdateAsync(Script entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Script>>> AddOrUpdate(IEnumerable<Script> items);

    ValueTask DeleteAllAsync(IEnumerable<Script> items);
}
