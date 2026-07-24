// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IScriptProcessingService
{
    Script GetScript(int scriptId);

    IQueryable<Script> GetAllScript(bool ignoreFilters = false);

    ValueTask<Script> AddScriptAsync(Script newScript);

    ValueTask<Script> UpdateScriptAsync(Script updatedScript);

    ValueTask DeleteAsync(int scriptId);

    ValueTask<IEnumerable<Result<Script>>> AddOrUpdateScriptResult(IEnumerable<Script> newScript);

    ValueTask DeleteAllScriptAsync(IEnumerable<Script> deletedScript);
}