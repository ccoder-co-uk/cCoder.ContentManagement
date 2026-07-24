// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ScriptService(IScriptBroker scriptBroker, IAuthorizationBroker authorizationBroker) : IScriptService
{
    public Script GetScript(int scriptId, bool ignoreFilters = false)
    {
        ValidateId(scriptId: scriptId, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAllScript(ignoreFilters: true)
                .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);
        }

        Script script = GetAllScript()
            .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);

        if (script != null)
        {
            return script;
        }

        Script script2 = GetAllScript(ignoreFilters: true)
            .FirstOrDefault(predicate: (Script i) => i.Id == scriptId);

        if (script2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Script> GetAllScript(bool ignoreFilters = false) =>
        scriptBroker.GetAllScripts(ignoreFilters: ignoreFilters);

    public async ValueTask<Script> AddScriptAsync(Script script)
    {
        ValidateScript(script: script, parameterName: "script");
        authorizationBroker.Authorize(appId: script.AppId, privilege: "Script_create");
        Script newScript = CreateStorageScript(newScript: script);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newScript.CreatedOn = DateTimeOffset.UtcNow);
        newScript.CreatedBy = currentUserId;
        newScript.LastUpdated = now;
        newScript.LastUpdatedBy = currentUserId;
        Script result = await scriptBroker.AddScriptAsync(newScript: newScript);
        script.Id = result.Id;
        script.Name = result.Name;
        script.Description = result.Description;
        script.LastUpdated = result.LastUpdated;
        script.LastUpdatedBy = result.LastUpdatedBy;
        script.CreatedOn = result.CreatedOn;
        script.CreatedBy = result.CreatedBy;
        script.AppId = result.AppId;
        script.Key = result.Key;
        script.Content = result.Content;
        return script;
    }

    public async ValueTask<Script> UpdateScriptAsync(Script updatedScript)
    {
        ValidateScript(script: updatedScript, parameterName: "script");
        authorizationBroker.Authorize(appId: updatedScript.AppId, privilege: "Script_update");
        Script updateScript = CreateStorageScript(newScript: updatedScript);

        string currentUserId = authorizationBroker.GetCurrentUser()
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
    }

    public async ValueTask DeleteAsync(int scriptId)
    {
        ValidateId(scriptId: scriptId, parameterName: "id");
        Script script;

        try
        {
            script = GetScript(scriptId: scriptId);
        }
        catch (SecurityException)
        {
            script = GetScript(scriptId: scriptId, ignoreFilters: true);
        }

        if (script == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: script.AppId, privilege: "Script_delete");
        await scriptBroker.DeleteScriptAsync(deletedScript: CreateStorageScript(newScript: script));
    }

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
}