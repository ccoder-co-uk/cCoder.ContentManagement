using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Processings;

internal class ScriptProcessingService(IScriptService service) : IScriptProcessingService
{
    public Script Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Script> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

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

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Script>>> AddOrUpdate(IEnumerable<Script> items)
    {
        ValidateScripts(items, "items");
        List<cCoder.ContentManagement.Models.Result<Script>> results = new List<cCoder.ContentManagement.Models.Result<Script>>();
        foreach (Script item in items)
        {
            try
            {
                Script savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Script>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Script>
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
        {
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateScript(Script script, string parameterName)
    {
        if (script == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateScripts(IEnumerable<Script> scripts, string parameterName)
    {
        if (scripts == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
