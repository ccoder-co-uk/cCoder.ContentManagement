using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ScriptService(IScriptBroker scriptBroker, IAuthorizationBroker authorizationBroker) : IScriptService
{
    public Script Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Script i) => i.Id == id);
        }

        Script script = GetAll().FirstOrDefault((Script i) => i.Id == id);
        if (script != null)
        {
            return script;
        }
        Script script2 = GetAll(ignoreFilters: true).FirstOrDefault((Script i) => i.Id == id);
        if (script2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Script> GetAll(bool ignoreFilters = false)
    {
        return scriptBroker.GetAllScripts(ignoreFilters);
    }

    public async ValueTask<Script> AddAsync(Script script)
    {
        ValidateScript(script, "script");
        authorizationBroker.Authorize(script.AppId, "Script_create");
        Script newScript = CreateStorageScript(script);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newScript.CreatedOn = DateTimeOffset.UtcNow);
        newScript.CreatedBy = currentUserId;
        newScript.LastUpdated = now;
        newScript.LastUpdatedBy = currentUserId;
        Script result = await scriptBroker.AddScriptAsync(newScript);
        result.App = script.App;
        return result;
    }

    public async ValueTask<Script> UpdateAsync(Script script)
    {
        ValidateScript(script, "script");
        authorizationBroker.Authorize(script.AppId, "Script_update");
        Script updateScript = CreateStorageScript(script);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateScript.LastUpdated = now;
        updateScript.LastUpdatedBy = currentUserId;
        Script result = await scriptBroker.UpdateScriptAsync(updateScript);
        result.App = script.App;
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Script script;
        try
        {
            script = Get(id);
        }
        catch (SecurityException)
        {
            script = Get(id, ignoreFilters: true);
        }

        if (script == null)
        {
            return;
        }

        authorizationBroker.Authorize(script.AppId, "Script_delete");
        await scriptBroker.DeleteScriptAsync(CreateStorageScript(script));
    }

    private static Script CreateStorageScript(Script script)
    {
        if (script == null)
        {
            return null;
        }

        return new Script
        {
            Id = script.Id,
            Name = script.Name,
            Description = script.Description,
            LastUpdated = script.LastUpdated,
            LastUpdatedBy = script.LastUpdatedBy,
            CreatedOn = script.CreatedOn,
            CreatedBy = script.CreatedBy,
            Key = script.Key,
            AppId = script.AppId,
            Content = script.Content
        };
    }
}
