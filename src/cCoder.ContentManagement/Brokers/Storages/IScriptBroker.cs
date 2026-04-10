using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IScriptBroker
{
    IQueryable<Script> GetAllScripts(bool ignoreFilters);

    ValueTask<Script> AddScriptAsync(Script entity);

    ValueTask<Script> UpdateScriptAsync(Script entity);

    ValueTask<int> DeleteScriptAsync(Script entity);

    ValueTask DeleteAllScriptsAsync(IEnumerable<Script> items);
}
