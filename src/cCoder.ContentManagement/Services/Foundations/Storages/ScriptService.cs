// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ScriptService(IScriptBroker scriptBroker, IAuthorizationManager authorizationManager) : IScriptService
{
    public Script GetScript(int scriptId, bool ignoreFilters = false) =>
        TryCatch<Script>(operation: () =>
    {
        ValidateScriptOnGet(inputs: [scriptId, ignoreFilters]);
        ValidateId(scriptId: scriptId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllScript(ignoreFilters: true)
                .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);
        }

        Script script = ExecuteGetAllScript()
            .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);

        if (script != null)
        {
            return script;
        }

        Script script2 = ExecuteGetAllScript(ignoreFilters: true)
            .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);

        if (script2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Script> GetAllScript(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Script>>(operation: () =>
    {
        ValidateAllScriptOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? scriptBroker.GetAllScriptsIgnoringFilters()
            : scriptBroker.GetAllScripts();
    });

    public ValueTask<Script> AddScriptAsync(Script newScript) =>
        TryCatch<Script>(operation: async () =>
    {
        ValidateScriptOnAdd(inputs: [newScript]);
        ValidateScript(script: newScript, parameterName: "script");
        authorizationManager.Authorize(appId: newScript.AppId, privilege: "Script_create");
        Script storageScript = CreateStorageScript(newScript: newScript);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storageScript.CreatedOn = DateTimeOffset.UtcNow);
        storageScript.CreatedBy = currentUserId;
        storageScript.LastUpdated = now;
        storageScript.LastUpdatedBy = currentUserId;
        Script result = await scriptBroker.AddScriptAsync(newScript: storageScript);
        newScript.Id = result.Id;
        newScript.Name = result.Name;
        newScript.Description = result.Description;
        newScript.LastUpdated = result.LastUpdated;
        newScript.LastUpdatedBy = result.LastUpdatedBy;
        newScript.CreatedOn = result.CreatedOn;
        newScript.CreatedBy = result.CreatedBy;
        newScript.AppId = result.AppId;
        newScript.Key = result.Key;
        newScript.Content = result.Content;
        return newScript;

    }, isValueTask: true);

    public ValueTask<Script> UpdateScriptAsync(Script updatedScript) =>
        TryCatch<Script>(operation: async () =>
    {
        ValidateScriptOnUpdate(inputs: [updatedScript]);
        ValidateScript(script: updatedScript, parameterName: "script");
        authorizationManager.Authorize(appId: updatedScript.AppId, privilege: "Script_update");
        Script updateScript = CreateStorageScript(newScript: updatedScript);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateScript.LastUpdated = now;
        updateScript.LastUpdatedBy = currentUserId;
        Script result = await scriptBroker.UpdateScriptAsync(updatedScript: updateScript);
        updatedScript.Id = result.Id;
        updatedScript.Name = result.Name;
        updatedScript.Description = result.Description;
        updatedScript.LastUpdated = result.LastUpdated;
        updatedScript.LastUpdatedBy = result.LastUpdatedBy;
        updatedScript.CreatedOn = result.CreatedOn;
        updatedScript.CreatedBy = result.CreatedBy;
        updatedScript.AppId = result.AppId;
        updatedScript.Key = result.Key;
        updatedScript.Content = result.Content;
        return updatedScript;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int scriptId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [scriptId]);
        ValidateId(scriptId: scriptId, parameterName: "id");
        Script script;

        try
        {
            script = ExecuteGetScript(scriptId: scriptId);
        }
        catch (SecurityException)
        {
            script = ExecuteGetScript(scriptId: scriptId, ignoreFilters: true);
        }

        if (script == null)
        {
            return;
        }

        authorizationManager.Authorize(appId: script.AppId, privilege: "Script_delete");
        await scriptBroker.DeleteScriptAsync(deletedScript: CreateStorageScript(newScript: script));

    }, isValueTask: true);

    private static Script CreateStorageScript(Script newScript)
    {
        if (newScript == null)
        {
            return null;
        }

        return new Script
        {
            Id = newScript.Id,
            Name = newScript.Name,
            Description = newScript.Description,
            LastUpdated = newScript.LastUpdated,
            LastUpdatedBy = newScript.LastUpdatedBy,
            CreatedOn = newScript.CreatedOn,
            CreatedBy = newScript.CreatedBy,
            Key = newScript.Key,
            AppId = newScript.AppId,
            Content = newScript.Content
        };
    }

    private IQueryable<Script> ExecuteGetAllScript(bool ignoreFilters = false) =>
        (ignoreFilters
            ? scriptBroker.GetAllScriptsIgnoringFilters()
            : scriptBroker.GetAllScripts());

    private Script ExecuteGetScript(int scriptId, bool ignoreFilters = false)
    {
        ValidateId(scriptId: scriptId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllScript(ignoreFilters: true)
                .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);
        }

        Script script = ExecuteGetAllScript()
            .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);

        if (script != null)
        {
            return script;
        }

        Script script2 = ExecuteGetAllScript(ignoreFilters: true)
            .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);

        if (script2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}