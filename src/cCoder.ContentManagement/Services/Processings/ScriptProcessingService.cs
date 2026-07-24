// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class ScriptProcessingService(IScriptService service) : IScriptProcessingService
{
    public Script Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Script> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Script> AddAsync(Script entity)
    {
        ValidateScript(script: entity, parameterName: "entity");
        return service.AddAsync(script: entity);
    }

    public ValueTask<Script> UpdateAsync(Script entity)
    {
        ValidateScript(script: entity, parameterName: "entity");
        return service.UpdateAsync(script: entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<Script>>> AddOrUpdate(IEnumerable<Script> items)
    {
        ValidateScripts(scripts: items, parameterName: "items");
        List<Result<Script>> results = new List<Result<Script>>();

        foreach (Script item in items)
        {
            try
            {
                Script savedItem = item.Id < 1 ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

                results.Add(item: new Result<Script>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Script>
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
        ValidateScripts(scripts: items, parameterName: "items");

        foreach (Script item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateScript(Script script, string parameterName) =>
        ThrowIf(condition: script == null, message: parameterName + " is required.");

    private static void ValidateScripts(IEnumerable<Script> scripts, string parameterName) =>
        ThrowIf(condition: scripts == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}