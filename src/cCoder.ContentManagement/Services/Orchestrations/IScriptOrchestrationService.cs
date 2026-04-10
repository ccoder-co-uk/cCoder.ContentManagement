using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IScriptOrchestrationService
{
    Script Get(int id);

    IQueryable<Script> GetAll(bool ignoreFilters = false);

    ValueTask<Script> AddAsync(Script entity);

    ValueTask<Script> UpdateAsync(Script entity);

    ValueTask DeleteAsync(int id);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Script>>> AddOrUpdate(IEnumerable<Script> items);

    ValueTask ImportScriptsAsync(int appId, Script[] items);

    ValueTask DeleteAllAsync(IEnumerable<Script> items);
}
