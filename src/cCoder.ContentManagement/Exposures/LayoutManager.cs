// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class LayoutManager(ILayoutOrchestrationService service) : ILayoutManager
{
    public Layout GetLayout(int layoutId) =>
        service.GetLayout(layoutId: layoutId);

    public IQueryable<Layout> GetAllLayout(bool ignoreFilters = false) =>
        service.GetAllLayout(ignoreFilters: ignoreFilters);

    public ValueTask<Layout> AddLayoutAsync(Layout newLayout) =>
        service.AddLayoutAsync(newLayout: newLayout);

    public ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout) =>
        service.UpdateLayoutAsync(updatedLayout: updatedLayout);

    public ValueTask DeleteAsync(int layoutId) =>
        service.DeleteAsync(layoutId: layoutId);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<OperationResult<Layout>>> AddOrUpdateLayoutResult(
        IEnumerable<Layout> newLayout) =>
        service.AddOrUpdateLayoutResult(newLayout: newLayout);

    public ValueTask ImportLayoutsAsync(int appId, Layout[] items) =>
        service.ImportLayoutsAsync(appId: appId, items: items);

    public ValueTask DeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout) =>
        service.DeleteAllLayoutAsync(deletedLayout: deletedLayout);
}