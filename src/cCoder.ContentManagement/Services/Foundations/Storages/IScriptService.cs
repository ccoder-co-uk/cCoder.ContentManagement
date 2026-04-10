using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IScriptService
{
    Script Get(int id, bool ignoreFilters = false);

    IQueryable<Script> GetAll(bool ignoreFilters = false);

    ValueTask<Script> AddAsync(Script script);

    ValueTask<Script> UpdateAsync(Script script);

    ValueTask DeleteAsync(int id);
}
