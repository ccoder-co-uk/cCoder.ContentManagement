// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ScriptManager(IScriptOrchestrationService service) : IScriptManager
{
    public Script GetScript(int scriptId) =>
        service.GetScript(scriptId: scriptId);

    public IQueryable<Script> GetAllScript(bool ignoreFilters = false) =>
        service.GetAllScript(ignoreFilters: ignoreFilters);

    public ValueTask<Script> AddScriptAsync(Script newScript) =>
        service.AddScriptAsync(newScript: newScript);

    public ValueTask<Script> UpdateScriptAsync(Script updatedScript) =>
        service.UpdateScriptAsync(updatedScript: updatedScript);

    public ValueTask DeleteAsync(int scriptId) =>
        service.DeleteAsync(scriptId: scriptId);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<OperationResult<Script>>> AddOrUpdateScriptResult(
        IEnumerable<Script> newScript) =>
        service.AddOrUpdateScriptResult(newScript: newScript);

    public ValueTask ImportScriptsAsync(int appId, Script[] items) =>
        service.ImportScriptsAsync(appId: appId, items: items);

    public ValueTask DeleteAllScriptAsync(IEnumerable<Script> deletedScript) =>
        service.DeleteAllScriptAsync(deletedScript: deletedScript);
}