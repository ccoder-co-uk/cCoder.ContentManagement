// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IScriptOrchestrationService
{
    Script GetScript(int scriptId);

    IQueryable<Script> GetAllScript(bool ignoreFilters = false);

    ValueTask<Script> AddScriptAsync(Script newScript);

    ValueTask<Script> UpdateScriptAsync(Script updatedScript);

    ValueTask DeleteAsync(int scriptId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Script>>> AddOrUpdateScriptResult(IEnumerable<Script> newScript);

    ValueTask ImportScriptsAsync(int appId, Script[] items);

    ValueTask DeleteAllScriptAsync(IEnumerable<Script> deletedScript);
}