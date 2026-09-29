// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IScriptBroker
{
    IQueryable<Script> GetAllScripts();

    IQueryable<Script> GetAllScriptsIgnoringFilters();

    ValueTask<Script> AddScriptAsync(Script newScript);

    ValueTask<Script> UpdateScriptAsync(Script updatedScript);

    ValueTask<int> DeleteScriptAsync(Script deletedScript);

    ValueTask DeleteAllScriptsAsync(IEnumerable<Script> deletedScript);
}