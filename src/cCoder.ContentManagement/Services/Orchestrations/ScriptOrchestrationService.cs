// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class ScriptOrchestrationService(
    IScriptProcessingService processingService,
    IScriptEventProcessingService eventService) : IScriptOrchestrationService
{
    public Script GetScript(int scriptId) =>
        processingService.GetScript(scriptId: ValidateId(scriptId: scriptId, parameterName: "id"));

    public IQueryable<Script> GetAllScript(bool ignoreFilters = false) =>
        processingService.GetAllScript(ignoreFilters: ignoreFilters);

    public async ValueTask<Script> AddScriptAsync(Script newScript)
    {
        ValidateScript(script: newScript, parameterName: "entity");

        Script result = await processingService.AddScriptAsync(newScript: newScript);
        await eventService.RaiseScriptAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Script> UpdateScriptAsync(Script updatedScript)
    {
        ValidateScript(script: updatedScript, parameterName: "entity");

        Script result = await processingService.UpdateScriptAsync(updatedScript: updatedScript);
        await eventService.RaiseScriptUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int scriptId)
    {
        ValidateId(scriptId: scriptId, parameterName: "id");

        Script entity;

        try
        {
            entity = processingService.GetScript(scriptId: scriptId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllScript(ignoreFilters: true)
                .FirstOrDefault(predicate: script => script.Id == scriptId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseScriptDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(scriptId: scriptId);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Script[] scriptsToDelete = [.. GetAllScript(ignoreFilters: true)
            .Where(predicate: script => script.AppId == appId)];

        if (scriptsToDelete.Length > 0)
        {
            await DeleteAllScriptAsync(deletedScript: scriptsToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Script>>> AddOrUpdateScriptResult(IEnumerable<Script> newScript)
    {
        Script[] scripts = ValidateScripts(scripts: newScript, parameterName: "items")
            .ToArray();

        List<Result<Script>> results = new();

        foreach (Script script in scripts)
        {
            try
            {
                Script result = script.Id <= 0
                    ? await AddScriptAsync(newScript: script)
                    : await UpdateScriptAsync(updatedScript: script);

                results.Add(item: new Result<Script>
                {
                    Success = true,
                    Item = result,
                    Message = script.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Script>
                {
                    Success = false,
                    Item = script,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask ImportScriptsAsync(int appId, Script[] items)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Script[] validatedItems = ValidateScripts(scripts: items, parameterName: "items")
            .ToArray();

        string[] names = validatedItems.Select(selector: script => script.Name.ToLower())
            .ToArray();

        var dbVersions = processingService.GetAllScript()
            .Where(predicate: script => script.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: script.Name.ToLower()))
            .Select(selector: script => new { script.Id, script.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: script =>
        {
            script.AppId = appId;

            script.Id = dbVersions.FirstOrDefault(predicate: existing =>
                existing.Name.Equals(value: script.Name, comparisonType: StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdateScriptResult(newScript: validatedItems);
    }

    public async ValueTask DeleteAllScriptAsync(IEnumerable<Script> deletedScript)
    {
        Script[] scripts = ValidateScripts(scripts: deletedScript, parameterName: "items")
            .ToArray();

        foreach (Script script in scripts)
        {
            await DeleteAsync(scriptId: script.Id);
        }
    }

    private static int ValidateId(int scriptId, string parameterName)
    {
        if (scriptId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return scriptId;
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Script ValidateScript(Script script, string parameterName)
    {
        if (script == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return script;
    }

    private static IEnumerable<Script> ValidateScripts(IEnumerable<Script> scripts, string parameterName)
    {
        if (scripts == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return scripts;
    }
}