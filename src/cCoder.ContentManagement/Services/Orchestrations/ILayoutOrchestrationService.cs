// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ILayoutOrchestrationService
{
    Layout Get(int id);

    IQueryable<Layout> GetAll(bool ignoreFilters = false);

    ValueTask<Layout> AddAsync(Layout entity);

    ValueTask<Layout> UpdateAsync(Layout entity);

    ValueTask DeleteAsync(int id);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items);

    ValueTask ImportLayoutsAsync(int appId, Layout[] items);

    ValueTask DeleteAllAsync(IEnumerable<Layout> items);
}