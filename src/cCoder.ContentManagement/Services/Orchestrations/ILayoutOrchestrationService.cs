// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ILayoutOrchestrationService
{
    Layout GetLayout(int layoutId);
    IQueryable<Layout> GetAllLayouts(bool ignoreFilters = false);
    ValueTask<Layout> AddLayoutAsync(Layout newLayout);
    ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout);
    ValueTask DeleteAsync(int layoutId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<OperationResult<Layout>>> AddOrUpdateLayoutResult(IEnumerable<Layout> newLayout);
    ValueTask ImportLayoutsAsync(int appId, Layout[] items);
    ValueTask DeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout);
}