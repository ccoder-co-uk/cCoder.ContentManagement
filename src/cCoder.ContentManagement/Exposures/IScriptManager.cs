// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IScriptManager
{
    Script GetScript(int scriptId);

    IQueryable<Script> GetAllScript(bool ignoreFilters = false);

    ValueTask<Script> AddScriptAsync(Script newScript);

    ValueTask<Script> UpdateScriptAsync(Script updatedScript);

    ValueTask DeleteAsync(int scriptId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<OperationResult<Script>>> AddOrUpdateScriptResult(IEnumerable<Script> newScript);

    ValueTask ImportScriptsAsync(int appId, Script[] items);

    ValueTask DeleteAllScriptAsync(IEnumerable<Script> deletedScript);
}