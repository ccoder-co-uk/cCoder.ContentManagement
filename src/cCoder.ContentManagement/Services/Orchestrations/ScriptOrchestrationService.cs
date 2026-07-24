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
    public Script Get(int id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Script> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Script> AddAsync(Script entity)
    {
        ValidateScript(script: entity, parameterName: "entity");

        Script result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseScriptAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Script> UpdateAsync(Script entity)
    {
        ValidateScript(script: entity, parameterName: "entity");

        Script result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseScriptUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        Script entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: script => script.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseScriptDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Script[] scriptsToDelete = [.. GetAll(ignoreFilters: true)
            .Where(predicate: script => script.AppId == appId)];

        if (scriptsToDelete.Length > 0)
        {
            await DeleteAllAsync(items: scriptsToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Script>>> AddOrUpdate(IEnumerable<Script> items)
    {
        Script[] scripts = ValidateScripts(scripts: items, parameterName: "items")
            .ToArray();

        List<Result<Script>> results = new();

        foreach (Script script in scripts)
        {
            try
            {
                Script result = script.Id <= 0
                    ? await AddAsync(entity: script)
                    : await UpdateAsync(entity: script);

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

        var dbVersions = processingService.GetAll()
            .Where(predicate: script => script.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: script.Name.ToLower()))
            .Select(selector: script => new { script.Id, script.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: script =>
        {
            script.AppId = appId;

            script.Id = dbVersions.FirstOrDefault(predicate: existing =>
                existing.Name.Equals(value: script.Name, comparisonType: StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdate(items: validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Script> items)
    {
        Script[] scripts = ValidateScripts(scripts: items, parameterName: "items")
            .ToArray();

        foreach (Script script in scripts)
        {
            await DeleteAsync(id: script.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return id;
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