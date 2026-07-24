// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ScriptProcessingService(IScriptService service) : IScriptProcessingService
{
    public Script GetScript(int scriptId) =>
        TryCatch<Script>(operation: () =>
    {
        ValidateScriptOnGet(inputs: [scriptId]);
        ValidateId(scriptId: scriptId, parameterName: "id");
        return service.GetScript(scriptId: scriptId);

    });

    public IQueryable<Script> GetAllScript(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Script>>(operation: () =>
    {
        ValidateAllScriptOnGet(inputs: [ignoreFilters]);
        return service.GetAllScript(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Script> AddScriptAsync(Script newScript) =>
        TryCatch<Script>(operation: () =>
    {
        ValidateScriptOnAdd(inputs: [newScript]);
        ValidateScript(script: newScript, parameterName: "entity");
        return service.AddScriptAsync(newScript: newScript);

    }, isValueTask: true);

    public ValueTask<Script> UpdateScriptAsync(Script updatedScript) =>
        TryCatch<Script>(operation: () =>
    {
        ValidateScriptOnUpdate(inputs: [updatedScript]);
        ValidateScript(script: updatedScript, parameterName: "entity");
        return service.UpdateScriptAsync(updatedScript: updatedScript);

    }, isValueTask: true);

    public ValueTask DeleteAsync(int scriptId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [scriptId]);
        ValidateId(scriptId: scriptId, parameterName: "id");
        return service.DeleteAsync(scriptId: scriptId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<Script>>> AddOrUpdateScriptResult(IEnumerable<Script> newScript) =>
        TryCatch<IEnumerable<Result<Script>>>(operation: async () =>
    {
        ValidateOrUpdateScriptResultOnAdd(inputs: [newScript]);
        ValidateScripts(scripts: newScript, parameterName: "items");
        List<Result<Script>> results = new List<Result<Script>>();

        foreach (Script item in newScript)
        {
            try
            {
                Script savedItem = item.Id < 1 ? await ExecuteAddScriptAsync(newScript: item) : await ExecuteUpdateScriptAsync(updatedScript: item);

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

    }, isValueTask: true);

    public ValueTask DeleteAllScriptAsync(IEnumerable<Script> deletedScript) =>
        TryCatch(operation: async () =>
    {
        ValidateAllScriptOnDelete(inputs: [deletedScript]);
        ValidateScripts(scripts: deletedScript, parameterName: "items");

        foreach (Script item in deletedScript)
        {
            await ExecuteDeleteAsync(scriptId: item.Id);
        }

    }, isValueTask: true);

    private static void ValidateId(int scriptId, string parameterName) =>
        ThrowIf(condition: scriptId < 1, message: parameterName + " must be greater than 0.");

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

    private ValueTask<Script> ExecuteAddScriptAsync(Script newScript)
    {
        ValidateScript(script: newScript, parameterName: "entity");
        return service.AddScriptAsync(newScript: newScript);
    }

    private ValueTask ExecuteDeleteAsync(int scriptId)
    {
        ValidateId(scriptId: scriptId, parameterName: "id");
        return service.DeleteAsync(scriptId: scriptId);
    }

    private ValueTask<Script> ExecuteUpdateScriptAsync(Script updatedScript)
    {
        ValidateScript(script: updatedScript, parameterName: "entity");
        return service.UpdateScriptAsync(updatedScript: updatedScript);
    }
}