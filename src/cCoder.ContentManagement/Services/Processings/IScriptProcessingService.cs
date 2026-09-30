// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IScriptProcessingService
{
    Script GetScript(int scriptId);

    IQueryable<Script> GetAllScripts(bool ignoreFilters = false);

    ValueTask<Script> AddScriptAsync(Script newScript);

    ValueTask<Script> UpdateScriptAsync(Script updatedScript);

    ValueTask DeleteAsync(int scriptId);

    ValueTask<IEnumerable<OperationResult<Script>>> AddOrUpdateScriptResult(IEnumerable<Script> newScript);

    ValueTask DeleteAllScriptAsync(IEnumerable<Script> deletedScript);
}