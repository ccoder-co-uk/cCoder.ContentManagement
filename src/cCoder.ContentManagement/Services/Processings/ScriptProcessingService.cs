using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class ScriptProcessingService(IScriptService service) : IScriptProcessingService
{
    public Script Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Script> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<Script> AddAsync(Script entity)
    {
        ValidateScript(entity, "entity");
        return service.AddAsync(entity);
    }

    public ValueTask<Script> UpdateAsync(Script entity)
    {
        ValidateScript(entity, "entity");
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result<Script>>> AddOrUpdate(IEnumerable<Script> items)
    {
        ValidateScripts(items, "items");
        List<Result<Script>> results = new List<Result<Script>>();
        foreach (Script item in items)
        {
            try
            {
                Script savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<Script>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Script>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Script> items)
    {
        ValidateScripts(items, "items");
        foreach (Script item in items)
            await DeleteAsync(item.Id);
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateScript(Script script, string parameterName) =>
        ThrowIf(script == null, parameterName + " is required.");

    private static void ValidateScripts(IEnumerable<Script> scripts, string parameterName) =>
        ThrowIf(scripts == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
